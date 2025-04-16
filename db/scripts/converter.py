stations = [
                {
                    "title": "Горный институт",
                    "occupancy": 5,
                    "accesstype": "ACCESSIBLE",
                    "opentime": "05:30",
                    "closetime": "01:30"
                },
                {
                    "title": "Спасская",
                    "occupancy": 5,
                    "accesstype": "ACCESSIBLE",
                    "opentime": "05:30",
                    "closetime": "01:30"
                },
                {
                    "title": "Достоевская",
                    "occupancy": 5,
                    "accesstype": "ACCESSIBLE",
                    "opentime": "05:30",
                    "closetime": "01:30"
                },
                {
                    "title": "Лиговский проспект",
                    "occupancy": 5,
                    "accesstype": "ACCESSIBLE",
                    "opentime": "05:30",
                    "closetime": "01:30"
                },
                {
                    "title": "Площадь Александра Невского-2",
                    "occupancy": 5,
                    "accesstype": "ACCESSIBLE",
                    "opentime": "05:30",
                    "closetime": "01:30"
                },
                {
                    "title": "Новочеркасская",
                    "occupancy": 5,
                    "accesstype": "ACCESSIBLE",
                    "opentime": "05:30",
                    "closetime": "01:30"
                },
                {
                    "title": "Ладожская",
                    "occupancy": 5,
                    "accesstype": "ACCESSIBLE",
                    "opentime": "05:30",
                    "closetime": "01:30"
                },
                {
                    "title": "Проспект Большевиков",
                    "occupancy": 5,
                    "accesstype": "ACCESSIBLE",
                    "opentime": "05:30",
                    "closetime": "01:30"
                },
                {
                    "title": "Улица Дыбенко",
                    "occupancy": 5,
                    "accesstype": "ACCESSIBLE",
                    "opentime": "05:30",
                    "closetime": "01:30"
                }
            ]

for i in range(1, len(stations)):
    print("{")
    print(f"    \"from\": \"{stations[i - 1]["title"]}\",")
    print(f"    \"to\": \"{stations[i]["title"]}\",")
    print(f"    \"duration\": \"03:00\"")
    print("},")