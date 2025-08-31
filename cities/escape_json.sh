#!/bin/sh

GREEN='\033[0;32m'
RED='\033[0;31m'
NC='\033[0m'

if [ -z "$1" ]; then
  printf "${RED}[ОШИБКА]${NC}: не указан путь к JSON-файлу"
  echo "Использование: $0 <путь/к/файлу.json>"
  exit 1
fi

input_file="$1"

if [ ! -f "$input_file" ]; then
  printf "${RED}[ОШИБКА]${NC}: файл \"%s\" не найден\n" "$input_file"
  exit 1
fi

if ! jq -e . "$input_file" >/dev/null 2>&1; then
  printf "${RED}[ОШИБКА]${NC}: файл \"$%s\" содержит некорректный JSON\n" "$input_file"
  exit 1
fi

dir=$(dirname "$input_file")
filename=$(basename "$input_file")
name="${filename%.*}"

output_file="$dir/${name}.escaped.json"
escaped_content=$(jq -c '.' "$input_file" | sed 's/"/\\"/g')

final_json="\"$escaped_content\""

echo "$final_json" > "$output_file"

printf "${GREEN}[УСПЕХ]${NC}: выходной файл - \"%s\"\n" "$output_file"