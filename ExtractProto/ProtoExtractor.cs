using CommandLine;
using ProtoBuf;
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;

namespace ExtractProto
{
	class Options
	{
		[Option('a')]
		public string Assembly { get; set; }

		[Option('p')]
		public string Package { get; set; }

		[OptionArray('t')]
		public string[] Types { get; set; }
	}

	public class ProtoExtractor
	{
		private static readonly Regex ProtoBlock = new Regex("(message|enum)(.|\n)*?(\\})");
		private static readonly Regex DefaultEnum = new Regex("(\\[default = )[A-Z]\\w*?(\\];)");
		private static readonly MethodInfo GetProtoMethod = typeof(Serializer).GetMethod("GetProto");

		public static void Main(string[] args)
		{
			var options = new Options();
			CommandLine.Parser.Default.ParseArgumentsStrict(args, options);

			var asm = Assembly.LoadFile(options.Assembly);
			var text = "";
			foreach (var t in options.Types)
			{
				var type = asm.GetType(t);
				text += GetProtoMethod.MakeGenericMethod(type).Invoke(null, null);
			}
			text = RemoveDupsFixEnums(options.Package, text);

			Console.WriteLine(text);
		}

		private static string RemoveDupsFixEnums(string package, string all)
		{
			var set = new SortedSet<string>();
			var enums = new Dictionary<string, string>();

			foreach (var m in ProtoBlock.Matches(all))
			{
				var str = m.ToString();
				set.Add(str);

				// make a master list of all enum values
				if (str.StartsWith("enum", StringComparison.CurrentCulture))
				{
					foreach (var line in str.Split('\n'))
					{
						if (line.StartsWith(" ", StringComparison.CurrentCulture))
						{
							var v = line.Split('=')[0].Trim();
							var k = v.Replace("_", "");
							if (!enums.ContainsKey(k))
							{
								enums.Add(k, v);
							}
						}
					}
				}
			}

			all = "package " + package + ";\n\n";
			foreach (var s in set)
			{
				foreach (var line in s.Split('\n'))
				{
					var str = line;

					foreach (var m in DefaultEnum.Matches(str))
					{
						var key = m.ToString().Split(new char[] { ']', '=' })[1].Trim();
						str = str.Replace(key, enums[key]);
					}

					all += str + '\n';
				}
			}

			return all;
		}
	}
}
