#!/bin/sh

stdStationOccupancy=5
stdStationAccessType="ACCESSIBLE"
stdStationOpenTime="05:30"
stdStationCloseTime="01:30"

stdTransitionOccupancy=5
stdTransitionAccessType="ACCESSIBLE"
stdTransitionOpenTime="05:30"
stdTransitionCloseTime="01:30"

MainDir="data"

# Разбор параметров
while getopts ":c:h" opt; do
    case $opt in
        c)
            MainDir="$OPTARG$MainDir"
        ;;
        h) 
            echo "Использование: $0 [-c рабочая директория, город] [-h]"
            exit 0
        ;;
        \?) 
            echo "Неверный параметр: -$OPTARG" >&2
            exit 1
        ;;
    esac
done

StationsDir="$MainDir/stations"
TransionsDir="$MainDir/transitions"
TitleFile="$MainDir/title"
ResFile="$MainDir/../chart.json"

if [ ! -f $TitleFile ]; then
    printf "Файл %s не существует\n" "$TitleFile"
    exit 1
fi

CityTitle=$(sed -n '1p' "$TitleFile")  # Название города
ChartTitle=$(sed -n '2p' "$TitleFile")  # Название схемы метро

printf "{\n\t\"title\": \"%s\",\n\t\"city\": \"%s\",\n\t\"branches\": [\n" "$ChartTitle" "$CityTitle" > "$ResFile"

printf "\033[0mАнализ \033[1;36mBranches\033[0m по пути \"%s\":\n" "$StationsDir"

if [ ! -d $StationsDir ]; then
    printf "Директория \"%s\" не существует\n" "$StationsDir"
fi

fileCount=$(find "$StationsDir" -maxdepth 1 -type f -printf . | wc -c)

for file in $(ls -1 "$StationsDir" | sort -V); do
    file="$StationsDir/$file"

    printf "\t\t{\n" >> "$ResFile"

    if [ -f "$file" ]; then
        filename=$(basename "$file")
        printf "\033[0m[\033[3;35mПарсинг\033[0m] \033[1;33mStations\033[0m из файла: \"%s\"\n" "$filename"
        
        branchTitle=$(sed -n '1p' "$file")  # Название линии
        branchColor=$(sed -n '2p' "$file")  # Цвет (#DA2128)
        branchStamp=$(sed -n '3p' "$file")  # Время (00:05:00)
        branchLooped=$(sed -n '4p' "$file")  # Число (0)

        station_count=$(grep -c '.' "$file")
        station_count=$((station_count - 4))
        carried_count=0

        printf "\t\t\t\"title\": \"%s\",\n\t\t\t\"color\": \"%s\",\n\t\t\t\"accesstype\": \"ACCESSIBLE\",\n\t\t\t\"stations\": [\n" "$branchTitle" "$branchColor" >> "$ResFile"

        counter=0
        while IFS= read -r stationTitle || [ -n "$stationTitle" ]; do
            # Пропустить первые 4 строки
            counter=$((counter + 1))
            if [ $counter -le 4 ]; then
                continue
            fi

            printf "\t\t\t\t{\n\t\t\t\t\t\"title\": \"%s\",\n" "$stationTitle" >> "$ResFile"
            printf "\t\t\t\t\t\"occupancy\": %s,\n" "$stdStationOccupancy" >> "$ResFile"
            printf "\t\t\t\t\t\"accesstype\": \"%s\",\n" "$stdStationAccessType" >> "$ResFile"
            printf "\t\t\t\t\t\"opentime\": \"%s\",\n" "$stdStationOpenTime" >> "$ResFile"
            printf "\t\t\t\t\t\"closetime\": \"%s\"\n" "$stdStationCloseTime" >> "$ResFile"

            carried_count=$((carried_count + 1))

            if [ $carried_count -ne $station_count ]; then
                printf "\t\t\t\t},\n" >> "$ResFile"
            else
                printf "\t\t\t\t}\n" >> "$ResFile"
            fi
        done < "$file"

        printf "\t\t\t],\n\t\t\t\"railways\": [\n" >> "$ResFile"

        printf "\033[0m[\033[3;35mПарсинг\033[0m] \033[1;32mRailways\033[0m из файла: \"%s\"\n" "$filename"

        railway_count=$((station_count - 1))
        prevStationTitle=$(sed -n '5p' "$file")
        carried_count=0

        if [ "$branchLooped" -eq 1 ]; then
            railway_count=$((railway_count + 1))
        fi

        counter=0
        while IFS= read -r curStationTitle || [ -n "$curStationTitle" ]; do
            counter=$((counter + 1))
            if [ $counter -le 5 ]; then
                continue
            fi

            printf "\t\t\t\t{\n" >> "$ResFile"
            printf "\t\t\t\t\t\"from\": \"%s\",\n" "$prevStationTitle" >> "$ResFile"
            printf "\t\t\t\t\t\"to\": \"%s\",\n" "$curStationTitle" >> "$ResFile"
            printf "\t\t\t\t\t\"duration\": \"%s\"\n" "$branchStamp" >> "$ResFile"

            prevStationTitle="$curStationTitle"
            carried_count=$((carried_count + 1))

            if [ $carried_count -ne $railway_count ]; then
                printf "\t\t\t\t},\n" >> "$ResFile"
            else
                printf "\t\t\t\t}\n" >> "$ResFile"
            fi
        done < "$file"

        if [ "$branchLooped" -eq 1 ]; then
            firstStationTitle=$(sed -n '5p' "$file")

            printf "\t\t\t\t{\n" >> "$ResFile"
            printf "\t\t\t\t\t\"from\": \"%s\",\n" "$prevStationTitle" >> "$ResFile"
            printf "\t\t\t\t\t\"to\": \"%s\",\n" "$firstStationTitle" >> "$ResFile"
            printf "\t\t\t\t\t\"duration\": \"%s\"\n" "$branchStamp" >> "$ResFile"
            printf "\t\t\t\t}\n" >> "$ResFile"
        fi

        printf "\t\t\t]\n" >> "$ResFile"

    fi

    if [ "$fileCount" -gt 1 ]; then
        printf "\t\t},\n" >> "$ResFile"
    else
        printf "\t\t}\n" >> "$ResFile"
    fi

    fileCount=$((fileCount - 1))
done

printf "\r"
printf "\t],\n\t\"transitions\": [\n" >> "$ResFile"

printf "\033[0mАнализ \033[1;36mTransitions\033[0m по пути \"%s\":\n" "$TransionsDir"

if [ ! -d $TransionsDir ]; then
    printf "Директория %s не существует\n" "$TransionsDir"
fi

for file in $(ls -1 "$TransionsDir" | sort -V); do
    curfile="$TransionsDir/$file"

    if [ -f "$curfile" ]; then
        filename=$(basename "$curfile")
        printf "\033[0m[\033[3;35mПарсинг\033[0m] \033[1;34mTransitons\033[0m из файла: \"%s\"\n" "$filename"

        if [ ! -f "$StationsDir/$file" ]; then
            printf "[\033[1;31mОшибка\033[0m] не удалось найти файл \"%s\", гарантированно составляющийся из имени \"%s\" из пути \"%s\" в \"%s\"\n" "$StationsDir/$file" "$file" "$curfile" "$StationsDir/$file"
            continue
        fi

        srcBranchTitle=$(sed -n '1p' "$StationsDir/$file")

        while IFS=';' read -r srcStationTitle dstStationFileTile dstStationTitle Interval || [ -n "$Connection" ]; do
            # printf "%s %s %s %s\n" "$srcStationTitle" "$dstStationFileTile" "$dstStationTitle" "$Interval"

            if printf "%s\n%s" "$filename" "$dstStationFileTile" | sort -V | head -1 | grep -qx "$dstStationFileTile"; then
                printf "[\033[1;31mОшибка\033[0m] задан маршрут из ветки \"%s\" в ветку \"%s\", где \"%s\" > \"%s\" по естественной сортировке\n" "$filename" "$dstStationFileTile" "$filename" "$dstStationFileTile"
                continue
            fi

            if ! grep -q "$srcStationTitle" "$StationsDir/$file"; then
                printf "[\033[1;31mОшибка\033[0m] не удалось найти станцию \"\033[1;34m%s\033[0m\" в ветке \"\033[1;34m%s\033[0m\"\n" "$srcStationTitle" "$srcBranchTitle"
                continue
            fi

            if [ ! -f "$StationsDir/$dstStationFileTile" ]; then
                printf "[\033[1;31mОшибка\033[0m] не удалось найти файл \"%s\", гарантированно составляющийся из имени \"%s\" из пути \"%s\" в \"%s\"\n" "$StationsDir/$dstStationFileTile" "$dstStationFileTile" "$TransionsDir/$dstStationFileTile" "$StationsDir/$dstStationFileTile"
                continue
            fi

            dstBranchTitle=$(sed -n '1p' "$StationsDir/$dstStationFileTile")

            if ! grep -q "$dstStationTitle" "$StationsDir/$dstStationFileTile"; then
                printf "[\033[1;31mОшибка\033[0m] не удалось найти станцию \"\033[1;34m%s\033[0m\" в ветке \"\033[1;34m%s\033[0m\"\n" "$dstStationTitle" "$dstBranchTitle"
                continue
            fi

            printf "\t\t{\n" >> "$ResFile"
            printf "\t\t\t\"occupancy\": %s,\n" "$stdTransitionOccupancy" >> "$ResFile"
            printf "\t\t\t\"accesstype\": \"%s\",\n" "$stdTransitionAccessType" >> "$ResFile"
            printf "\t\t\t\"duration\": \"%s\",\n" "$Interval" >> "$ResFile"
            printf "\t\t\t\"opentime\": \"%s\",\n" "$stdTransitionOpenTime" >> "$ResFile"
            printf "\t\t\t\"closetime\": \"%s\",\n" "$stdTransitionCloseTime" >> "$ResFile"
            printf "\t\t\t\"branchsrc\": \"%s\",\n" "$srcBranchTitle" >> "$ResFile"
            printf "\t\t\t\"stationsrc\": \"%s\",\n" "$srcStationTitle" >> "$ResFile"
            printf "\t\t\t\"branchdst\": \"%s\",\n" "$dstBranchTitle" >> "$ResFile"
            printf "\t\t\t\"stationdst\": \"%s\"\n" "$dstStationTitle" >> "$ResFile"
            printf "\t\t},\n" >> "$ResFile"

        done < "$curfile"
    fi
done

printf "\t]\n}" >> "$ResFile"


# Убрать ненужную запятую
line=$(($(wc -l < "$ResFile") - 1))
sed -i "${line}s/.*/\t\t}/" "$ResFile"