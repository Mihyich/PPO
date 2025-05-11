from pathlib import Path
from itertools import takewhile
import shutil
import json
import psycopg2


workTempDir = Path("./TempData")
chartFile = "/home/mihail/Рабочий стол/BMSTU/PPO_BACKUP/cities/Moscow/chart.json"


dbname = "metro"
user = "postgres"
password = "1234"
host = "localhost"


def hexColorToDecimal(color: str) -> int:
    hc = color.lstrip('#')

    if len(hc) == 3:
        hc = ''.join([c * 2 for c in hc])

    return int(hc, 16)


getStationSerialIndexInsideLocalBranch = lambda bTitle, sTitle, data: \
    next((i for i, s in enumerate((next((b for b in data['branches'] if b['title'] == bTitle), None))['stations']) if s['title'] == sTitle), None)


getAmountOfStationsInBranchesInSeqOrderBeforeTheBranch = lambda bTitle, data: \
    sum(len(b['stations']) for b in takewhile(lambda x: x['title'] != bTitle, data['branches']))


getStationSerialIndex = lambda bTitle, sTitle, data: \
    getAmountOfStationsInBranchesInSeqOrderBeforeTheBranch(bTitle, data) + \
    getStationSerialIndexInsideLocalBranch(bTitle, sTitle, data)


def readJsonFile(file: str):
    try:
        with open(chartFile, 'r', encoding='utf-8') as file:
            data = json.load(file)
    except FileNotFoundError:
        print("[Ошибка] Файл не найден")
        exit(1)
    except json.JSONDecodeError:
        print("[Ошибка] Некорректный формат JSON")
        exit(1)

    return data


def getNextPrimaryKeyValue(chart: str, key: str) -> int:
    conn = psycopg2.connect(dbname=dbname, user=user, password=password, host=host)

    with conn:
        with conn.cursor() as curs:
            # Имя sequence
            curs.execute(f"SELECT pg_get_serial_sequence('{chart}', '{key}');")
            seq_name = curs.fetchone()[0]

            # Следующее значение ключа без увелечения его счетчика
            if seq_name:
                curs.execute(
                    f"""
                    SELECT COALESCE(
                        (SELECT last_value + 1 
                         FROM pg_sequences 
                         WHERE sequencename = %s AND schemaname = 'public'),
                        1
                    );
                    """, (seq_name.split('.')[-1],))  # Убрать имя схемы из имени, если есть
                next_id = curs.fetchone()[0]

    conn.close()

    return next_id


def involuteDirectories():
    workTempDir.mkdir(exist_ok=True)


def convoluteDirectories():
    if workTempDir.exists():
        shutil.rmtree(workTempDir)


def createChartCSV(data):
    fileName = workTempDir / Path("chart.csv")

    with open(fileName, "w", encoding='utf-8') as f:
        print(f"{data['title']};{data['city']}", file=f)


def createBranchCSV(data):
    fileName = workTempDir / Path("branch.csv")

    with open(fileName, "w", encoding='utf-8') as f:
        for b in data['branches']:
            print(f"{b['title']};{hexColorToDecimal(b['color'])};{b['accesstype']}", file=f)


def createStationCSV(data):
    fileName = workTempDir / Path("station.csv")

    with open(fileName, "w", encoding='utf-8') as f:
        for b in data['branches']:
            for s in b['stations']:
                print(f"{s['title']};{s['occupancy']};{s['accesstype']};{s['opentime']};{s['closetime']}", file=f)


def createTransitionCSV(data):
    fileName = workTempDir / Path("transition.csv")

    with open(fileName, "w", encoding='utf-8') as f:
        for t in data['transitions']:
            print(f"{t['occupancy']};{t['accesstype']};{t['duration']};{t['opentime']};{t['closetime']}", file=f)


def createChartBranchCSV(data):
    fileName = workTempDir / Path("chart_branch.csv")
    chartNextId = getNextPrimaryKeyValue("chart", "id")
    branchNextId = getNextPrimaryKeyValue("branch", "id")

    with open(fileName, "w", encoding='utf-8') as f:
        for _ in range(0, len(data['branches'])):
            print(f"{chartNextId};{branchNextId}", file=f)
            branchNextId += 1


def createBranchStationCSV(data):
    fileName = workTempDir / Path("branch_station.csv")
    branchNextId = getNextPrimaryKeyValue("branch", "id")
    stationNextId = getNextPrimaryKeyValue("station", "id")

    with open(fileName, "w", encoding='utf-8') as f:
        for b in data['branches']:
            for _ in range(0, len(b['stations'])):
                print(f"{branchNextId};{stationNextId}", file=f)
                stationNextId += 1

            branchNextId += 1


def createRailwayCSV(data):
    fileName = workTempDir / Path("railway.csv")
    stationNextId = getNextPrimaryKeyValue("station", "id")

    with open(fileName, "w", encoding='utf-8') as f:
        for b in data['branches']:
            for r in b['railways']:
                from_id = stationNextId + next((i for i, s in enumerate(b['stations']) if s['title'] == r['from']), None)
                to_id = stationNextId + next((i for i, s in enumerate(b['stations']) if s['title'] == r['to']), None)
                duration = r['duration']
                print(f"{from_id};{to_id};{duration}", file=f)

            stationNextId += len(b['railways'])


def createStationTransition(data):
    fileName = workTempDir / Path("station_transition.csv")
    stationNextId = getNextPrimaryKeyValue("station", "id")
    transitionNextId = getNextPrimaryKeyValue("transition", "id")

    with open(fileName, "w", encoding='utf-8') as f:
        for t in data['transitions']:
            bsrc, ssrc = t["branchsrc"], t["stationsrc"]
            bdst, sdst = t["branchdst"], t["stationdst"]

            srcId = stationNextId + getStationSerialIndex(bsrc, ssrc, data)
            dstId = stationNextId + getStationSerialIndex(bdst, sdst, data)

            print(f"{srcId};{transitionNextId}\n{dstId};{transitionNextId}", file=f)

            transitionNextId += 1


def main():
    data = readJsonFile(chartFile)
    involuteDirectories()

    createChartCSV(data)
    createBranchCSV(data)
    createStationCSV(data)
    createTransitionCSV(data)

    createChartBranchCSV(data)
    createBranchStationCSV(data)
    createRailwayCSV(data)
    createStationTransition(data)

    convoluteDirectories()
    exit(0)


if __name__ == '__main__':
    main()