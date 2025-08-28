#!/bin/sh

GREEN='\033[0;32m'
RED='\033[0;31m'
NC='\033[0m'

if [ -z "$1" ]; then
  echo "${RED}[ОШИБКА]${NC}: не указан путь к JSON-файлу"
  echo "Использование: $0 <путь/к/файлу.json>"
  exit 1
fi

input_file="$1"

if [ ! -f "$input_file" ]; then
  echo "${RED}[ОШИБКА]${NC}: файл \"${input_file}\" не найден"
  exit 1
fi

if ! jq -e . "$input_file" >/dev/null 2>&1; then
  echo "${RED}[ОШИБКА]${NC}: файл \"${input_file}\" содержит некорректный JSON"
  exit 1
fi

dir=$(dirname "$input_file")
filename=$(basename "$input_file")
name="${filename%.*}"

output_file="$dir/${name}.escaped.json"
escaped_content=$(jq -c '.' "$input_file" | sed 's/"/\\"/g')

final_json="\"$escaped_content\""

echo "$final_json" > "$output_file"

echo "${GREEN}[УСПЕХ]:${NC} выходной файл - \"${output_file}\""