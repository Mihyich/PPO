#!/bin/sh

load_chart() {
    echo "Загрузка схемы \"$1\""
    python3.12 ./addChart.py --chart "$1"
}

load_chart "./../../cities/Adana/chart.json"
load_chart "./../../cities/Minsk/chart.json"
load_chart "./../../cities/Moscow/chart.json"
load_chart "./../../cities/Sankt-Peterburg/chart.json"