#!/bin/bash

WORKSPACE=..
LUBAN_DLL=$WORKSPACE/Tools/Luban/Luban.dll
CONF_ROOT=.
GAMEDATA=%WORKSPACE/Assets/GameData

dotnet $LUBAN_DLL \
    -t all \
    -d json \
    --conf $CONF_ROOT/luban.conf \
    -x outputDataDir=output

# 将 json 文件名首字母改为大写
for f in ../Assets/GameData/*.json; do
  filename=$(basename "$f")
  newname="$(echo ${filename:0:1} | tr '[:lower:]' '[:upper:]')${filename:1}"
  [ "$filename" != "$newname" ] && mv "$f" "$(dirname "$f")/$newname"
done