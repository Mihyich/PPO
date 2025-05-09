import requests
from bs4 import BeautifulSoup
import os, shutil, time

url = 'https://yandex.ru/metro/moscow?scheme_id=sc34974011'
directory = 'temp_cities'
cur_dir = ''

response = requests.get(url)

if response.status_code == 200:
    soup = BeautifulSoup(response.text, 'html.parser')
    container = soup.select_one('body > div.body > div > div.metro-app__sidebar-container > div.metro-header-view > div.metro-header-view__controls-container > div > div > div > div > div > div')

    if container:
        links = container.find_all('a')

        if os.path.exists(directory):
            shutil.rmtree(directory)
        os.makedirs(directory)

        for link in links:
            cur_dir = directory + '/' + link.text.strip()
            os.makedirs(cur_dir)

            print(f"Создана директория: {cur_dir}")
            print(f"    Переход по ссылке: {link['href']}")

            city_response = requests.get(link['href'])
            
            if city_response.status_code == 200:
                city_soup = BeautifulSoup(city_response.text, 'html.parser')

                time.sleep(1)

                svgs = []
                for i in range(1, 4):
                    svg_selector = f'body > div.body > div > div.metro-scheme-container > div.metro-scheme-view._visible > div > div > svg:nth-child({i})'
                    svg = city_soup.select_one(svg_selector)
                    if svg:
                        svgs.append(svg)
                    else:
                        print("FAIL")   
            else:
                print(f'    Ошибка при запросе: {city_response.status_code}')
    else:
        print("Контейнер с городами не найден.")
else:
    print(f'Ошибка при запросе: {response.status_code}')