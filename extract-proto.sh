#!/usr/bin/env bash

while [[ $# -gt 1 ]]
do
KEY="$1"

case ${KEY} in
    -a)
    ASSEMBLY="$2"
    shift
    ;;
    -p)
    PACKAGE="$2"
    shift
    ;;
    -t)
    TYPES="${@:2}"
    ;;
    *)
    # unknown option
    ;;
esac
shift
done

docker build -q -t squishly/extract-protobuf-net . > /dev/null 2>&1
docker run \
  --interactive --tty --rm \
  --volume ${ASSEMBLY}:/asm \
  squishly/extract-protobuf-net "-a /asm -p ${PACKAGE} -t ${TYPES}"
