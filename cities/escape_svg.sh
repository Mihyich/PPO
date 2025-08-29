#!/bin/bash

GREEN='\033[0;32m'
RED='\033[0;31m'
NC='\033[0m'

if [ -z "$1" ]; then
  echo "${RED}[ОШИБКА]${NC}: не указан путь к SVG-файлу"
  echo "Использование: $0 <путь/к/файлу.svg>"
  exit 1
fi

input_file="$1"

if [ ! -f "$input_file" ]; then
  echo "${RED}[ОШИБКА]${NC}: файл \"$input_file\" не найден"
  exit 1
fi

dir=$(dirname "$input_file")
filename=$(basename "$input_file")
name="${filename%.*}"

output_file="$dir/${name}.escaped.svg"
escaped_content=$(cat "$input_file" | sed 's/"/\\"/g' | tr -d '\n' | tr -d '\r')

final_svg="\"$escaped_content\""

echo "$final_svg" > "$output_file"

echo "${GREEN}[УСПЕХ]:${NC} выходной файл - \"${output_file}\""