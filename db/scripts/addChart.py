from pathlib import Path
import shutil
import json


workTempDir = Path("./TempData")
chartFile = "/home/mihail/Рабочий стол/BMSTU/PPO_BACKUP/cities/Moscow/chart.json"


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
            print(f"{b['title']};{b['color']};{b['accesstype']}", file=f)


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

def main():
    data = readJsonFile(chartFile)
    involuteDirectories()

    createChartCSV(data)
    createBranchCSV(data)
    createStationCSV(data)
    createTransitionCSV(data)

    convoluteDirectories()
    exit(0)


if __name__ == '__main__':
    main()