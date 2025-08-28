import argparse
from pathlib import Path
from lxml import etree

# file_path_scheme = "/home/mihail/Рабочий стол/BMSTU/PPO/SvgChartParser/test1.svg"
# file_path_titles = "/home/mihail/Рабочий стол/BMSTU/PPO/SvgChartParser/test2.svg"

# file_res = "test1res.svg"

svg_ns = 'http://www.w3.org/2000/svg'
ns = {'svg': svg_ns}

def fix_namespaces(element, ns):
    if not isinstance(element, etree._Element):
        return

    if isinstance(element.tag, str) and '}' not in element.tag:
        element.tag = f'{{{ns}}}{element.tag}'

    for child in element:
        fix_namespaces(child, ns)


def find_railways_elements(root):
    railway_elements = root.xpath(
        './/svg:line | .//svg:path',
        namespaces=ns
    )

    railway_elements_color = {}
    for line in railway_elements:
        color = line.get('stroke')

        if color is None:
            color = 'none'

        if color not in railway_elements_color:
            railway_elements_color[color] = []
        
        railway_elements_color[color].append(line)

    return railway_elements, railway_elements_color


def find_stations_elements(root):
    station_elements = root.xpath(
        '//svg:g[@class="scheme-objects-view__station"]//svg:circle',
        namespaces=ns
    )

    station_elements_color = {}
    for station in station_elements:
        color = station.get('fill')

        if color is None:
            color = 'none'

        if color not in station_elements_color:
            station_elements_color[color] = []
        
        station_elements_color[color].append(station)

    return station_elements, station_elements_color


def find_transition_elements(root):
    transition_elements = root.xpath(
        '//svg:g[@class="scheme-objects-view__transfers"]//svg:path',
        namespaces=ns
    )

    return transition_elements


def find_station_titles(root):
    station_titles_groups = root.xpath(
        '//svg:g[@class="scheme-objects-view__label"]',
        namespaces=ns
    )

    station_titles_elements = []
    for group in station_titles_groups:
        circle = group.xpath(
            './/svg:circle[@fill != "#ffffff"]',
            namespaces=ns
        )[0]

        text = group.xpath(
            './/svg:text',
            namespaces=ns
        )[0]

        text.attrib['stroke'] = circle.attrib['fill']
        text.attrib['circle_px'] = circle.attrib['x']
        text.attrib['circle_py'] = circle.attrib['y']

        # //svg:tspan/text()
        station_titles_elements.append(text)

    station_titles_elements_color = {}
    for station in station_titles_elements:
        color = station.get('stroke')

        if color is None:
            color = 'none'

        if color not in station_titles_elements_color:
            station_titles_elements_color[color] = []
        
        station_titles_elements_color[color].append(station)

    return station_titles_elements, station_titles_elements_color


def main():
    parser = argparse.ArgumentParser(description='Конвертация SVG схемы от Яндекс Метро')
    parser.add_argument('--scheme', type=Path, required=True, help='Путь к файлу scheme.json')
    parser.add_argument('--titles', type=Path, required=True, help='Путь к файлу titles.json')
    args = parser.parse_args()

    file_path_scheme = Path(args.scheme)
    file_path_titles = Path(args.titles)

    file_res = file_path_scheme.with_name("chart.svg")

    inserted_stations_cnt = 0

    parser = etree.XMLParser(load_dtd=False, no_network=True, resolve_entities=False)

    tree = etree.parse(file_path_scheme, parser)
    root_scheme = tree.getroot()

    tree = etree.parse(file_path_titles, parser)
    root_titles = tree.getroot()

    fix_namespaces(root_scheme, svg_ns)
    print(f"Исправлено пространство имен: {svg_ns} для {file_path_scheme}")

    fix_namespaces(root_titles, svg_ns)
    print(f"Исправлено пространство имен: {svg_ns} для {file_path_titles}")

    railway_elements, railway_elements_color = find_railways_elements(root_scheme)
    print(f"Найдено соединений: {len(railway_elements)}")
    print("Сортировка цветов:")
    for color, line_list in railway_elements_color.items():
        print(f"Цвет ветки: {color} --> {len(line_list)}")

    station_elements, station_elements_color = find_stations_elements(root_scheme)
    print(f"Найдено станций: {len(station_elements) / 2.0} реальных, {len(station_elements)} всего")
    print("Сортировка цветов:")    
    for color, station_list in station_elements_color.items():
        print(f"Цвет станции: {color} --> {len(station_list)}")

    station_titles_elements, station_titles_elements_color = find_station_titles(root_titles)
    print(f"Найдено имен станций: {len(station_titles_elements)}")
    print("Сортировка цветов:")    
    for color, station_list in station_titles_elements_color.items():
        print(f"Цвет станции: {color} --> {len(station_list)}")


    transition_elements = find_transition_elements(root_scheme)
    print(f"Найдено элементов для переходов: {len(transition_elements)}")


    new_root = etree.Element(f'{{{svg_ns}}}svg', nsmap={None: svg_ns})

    for attr in ['width', 'height', 'viewBox', 'xmlns']:
        if attr in root_scheme.attrib:
            new_root.set(attr, root_scheme.get(attr))

    print("Генерация SVG...")

    for color, line_list in railway_elements_color.items():
        if color in ['none', '#ffffff', '#000000', '#aaaaaa']:
            continue

        # Группа - ветка:соединители
        group_branch_railways = etree.SubElement(
            new_root,
            f'{{{svg_ns}}}g',
            attrib={
                'id': 'branch_railways',
                'stroke': color,
                'stroke-width': '8',
                'fill': 'none'
            }
        )

        for line in line_list:
            tag = etree.QName(line).localname
            line_copy = etree.Element(f'{{{svg_ns}}}{tag}')
            attrib = {}

            if tag == "line":
                attrib = {key: value for key, value in line.attrib.items() if key in ['x1', 'y1', 'x2', 'y2']}
            elif tag == "path":
                attrib = {key: value for key, value in line.attrib.items() if key in ['d']}

            line_copy.attrib.update(attrib)
            group_branch_railways.append(line_copy)


    # Группа - переходы
    group_transitions = etree.SubElement(
        new_root,
        f'{{{svg_ns}}}g',
        attrib={
            'id': 'transitions'
        }
    )

    branch_groups = new_root.xpath(
        '//svg:g[@id="branch_railways"]',
        namespaces=ns
    )

    for path in transition_elements:
        path_copy = etree.Element(f'{{{svg_ns}}}path')
        path_copy.attrib.update(path.attrib)
        group_transitions.append(path_copy)

        # Удаление повторяющихся path из соединителей
        target_d = path_copy.attrib["d"]
        for group in branch_groups:
            paths_to_remove = group.xpath(
                f'.//svg:path[@d="{target_d}"]',
                namespaces=ns
            )
            
            for path in paths_to_remove:
                group.remove(path)
                print(f"Удалён path с d='{target_d}' из группы {group.get('id')}")



    for color in railway_elements_color:
        if color in station_elements_color and color not in ['none', '#ffffff', '#000000', '#aaaaaa']:
            # Группа - ветка:станции
            group_branch_stations = etree.SubElement(
                new_root,
                f'{{{svg_ns}}}g',
                attrib={
                    'id': 'Название ветки'
                }
            )

            for station in station_elements_color[color]:
                # Подгруппа - станция
                group_station = etree.SubElement(
                    group_branch_stations,
                    f'{{{svg_ns}}}g',
                    attrib={
                        'id': 'Название станции',
                    }
                )

                attrib = {key: value for key, value in station.attrib.items() if key in ['x', 'y', 'cx', 'cy']}
                attrib['r'] = "5"
                attrib["fill"] = color
                circle_copy = etree.Element(f'{{{svg_ns}}}circle')
                circle_copy.attrib.update(attrib)

                attrib['r'] = "8"
                attrib["fill"] = "#ffffff"
                attrib["opacity"] = "1"
                circuit_copy = etree.Element(f'{{{svg_ns}}}circle')
                circuit_copy.attrib.update(attrib)

                group_station.append(circuit_copy)
                group_station.append(circle_copy)

                inserted_stations_cnt += 1

                if color in station_titles_elements_color:
                    x = station.attrib['x']
                    y = station.attrib['y']

                    fit_text_element = None
                    for el in station_titles_elements_color[color]:
                        if el.attrib['circle_px'] == x and el.attrib['circle_py'] == y:
                            fit_text_element = el
                            break
                    
                    if fit_text_element is not None:
                        # Подгруппа - имя станции
                        text_group = etree.SubElement(
                            group_station,
                            f'{{{svg_ns}}}text',
                            attrib={
                                'x': fit_text_element.attrib['x'],
                                'y': fit_text_element.attrib['y'],
                                'font-size': '20',
                                'font-weight': 'normal',
                                'text-anchor': fit_text_element.attrib['text-anchor'],
                                'opacity': 'unset'
                            }
                        )


                        full_title = ""
                        for tspan in fit_text_element.xpath(
                            './/svg:tspan',
                            namespaces=ns
                        ):
                            tspan_el = etree.Element(f'{{{svg_ns}}}tspan')
                            tspan_el.attrib.update(tspan.attrib)
                            tspan_el.text = tspan.text
                            text_group.append(tspan_el)
                            full_title += tspan.text + " "
                        
                        full_title = full_title.strip()
                        group_station.attrib['id'] = full_title
                        print(full_title)




    new_tree = etree.ElementTree(new_root)
    new_tree.write(file_res, pretty_print=True, xml_declaration=True, encoding='utf-8')

    print(f'Создано станций: {inserted_stations_cnt}')
    print(f'Мусорных станций: {len(station_elements) / 2.0 - inserted_stations_cnt}')

    print("Упех")


if __name__ == '__main__':
    main()