#!/bin/sh

printf "Введите имя создаваемой папки: "
read -r directory

if [ -d "$directory" ]; then
    printf "[\033[1;33mПредупреждение\033[0m]: Директория \"%s\" уже существует\n" "$directory"
    printf "Перезаписать (Да/Нет)?: "
    read -r ans
    if ! expr "$ans" : '[Дд][Аа]' > /dev/null; then
        exit 0
    fi
fi

printf "Введите название города: "
read -r city

printf "Введите название схемы: "
read -r title

printf "Введите количество веток в схеме: "
read -r branchCount

while [ ! "$(expr "$branchCount" : '^[0-9]\+$')" -gt 0 ]
do
    printf "[\033[1;31mОшибка\033[0m]: Требуется целое, положительное число\nВведите количество веток в схеме: "
    read -r branchCount
done

printf "[\033[3;35mГенерация\033[0m]: Создание корневой директории: \"./%s\"\n" "$directory"
mkdir -p "./$directory"

printf "[\033[3;35mГенерация\033[0m]: Создание директории данных: \"./%s/data\"\n" "$directory"
mkdir -p "./$directory/data"

printf "[\033[3;35mГенерация\033[0m]: Создание директории станций: \"./%s/data/stations\"\n" "$directory"
mkdir -p "./$directory/data/stations"

printf "[\033[3;35mГенерация\033[0m]: Создание директории переходов: \"./%s/data/transitions\"\n" "$directory"
mkdir -p "./$directory/data/transitions"

printf "[\033[3;35mГенерация\033[0m]: Создание заголовочного файла \"title\": \"./%s/data/title\"\n" "$directory"
printf "%s\n%s" "$city" "$title" > "./$directory/data/title"

printf "[\033[3;35mГенерация\033[0m]: Создание файлов, описавающие ветки и переходы\n"

i=1
while [ "$i" -le "$branchCount" ]
do
    printf "" > "./$directory/data/stations/$i"
    printf "" > "./$directory/data/transitions/$i"
    i=$((i+1))
done