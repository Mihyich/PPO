import os
from lxml import etree
import json
import re

directory = "./temp_cities"
svg_chart_path = os.path.abspath('./temp_cities/Sankt-Peterburg/chart.svg')
svg_titles_path = os.path.abspath('./temp_cities/Sankt-Peterburg/titles.svg')
local_directory = os.path.abspath('./temp_cities/Sankt-Peterburg/')

def extract_join_points(d: str):
    pattern = r'([MLAQL])\s*([\d\s.,-]+)'
    matches = re.findall(pattern, d)
    coords = []
    vals = None
    safe_convert = lambda v: float(v) if '.' in v else int(v)

    for command, numbers in matches:
        vals = [safe_convert(v.strip(',')) for v in numbers.strip().split()]

        if (command == 'M' or command == 'L'):
            coords.append({'x': vals[0], 'y': vals[1]})
        elif (command == 'A'):
            coords.append({'x': vals[5], 'y': vals[6]})
        elif (command == 'Q'):
            coords.append({'x': vals[2], 'y': vals[3]})

    unique_coords = [dict(t) for t in {tuple(d.items()) for d in coords}]
    return unique_coords

def extract_stations(svg):
    tree = etree.parse(svg)
    root = tree.getroot()

    stations = [{'x': st.attrib['x'], 'y': st.attrib['y']} for circle in
                [g.findall('.//circle[@fill]') + g.findall('.//tspan') for g in root.xpath('//g[@class]') if g.attrib['class'] in ['scheme-objects-view__label', 'scheme-objects-view__label _warning', 'scheme-objects-view__label _closed']]
                for st in circle if all(key in st.attrib for key in ['x', 'y', 'cx', 'cy', 'r', 'fill']) and st.attrib['fill'] != '#ffffff']

    titles = [g.findall('.//tspan')[0].text for g in root.xpath('//g[@class]') if g.attrib['class'] == 'scheme-objects-view__label']

    res = [{'x': st['x'], 'y': st['y'], 'title': title, 'occupancy': 5, 'access_type': 'ACCESSIBLE', 'open_time': '05:30:00', 'close_time': '00:52:00'} for st, title in zip(stations, titles)]

    for r in res:
        print(r)

    # for s, t in zip(stations, titles):
    #     print(s, t)

    return res

def extract_transfers(svg):
    tree = etree.parse(svg)
    root = tree.getroot()

    temp_group = [t for t in root.xpath('.//g[@class]') if t.attrib['class'] == 'scheme-objects-view__transfers']
    if len(temp_group):
        transfers = [extract_join_points(t.attrib['d']) for t in temp_group[0].findall('.//path[@stroke]') if t.attrib['stroke'] == '#aaaaaa']

    res = [{'joins': t, 'occupancy': 5, 'access_type': 'ACCESSIBLE', 'open_time': '05:30:00', 'close_time': '00:52:00'} for t in transfers]

    for r in res:
        print(r)

    # for t in transfers:
    #     print(t)

    return res

def extract_railways(svg):
    tree = etree.parse(svg)
    root = tree.getroot()

    pathes = [extract_join_points(p.attrib['d']) for p in root.findall('.//path[@stroke]') if all(k in p.attrib for k in ['d', 'stroke']) and p.attrib['stroke'] != '#ffffff']
    lines = [l.attrib for l in root.findall('.//line[@stroke]') if all(k in l.attrib for k in ['x1', 'x2', 'y1', 'y2', 'stroke']) and l.attrib['stroke'] != '#ffffff']

    res = [{'x1': j[0]['x'], 'y1': j[0]['y'], 'x2': j[-1]['x'], 'y2': j[-1]['y'], 'duration': '00:04:00'} for j in pathes]
    res += [{'x1': l['x1'], 'y1': l['y1'], 'x2': l['x2'], 'y2': l['y2'], 'duration': '00:04:00'} for l in lines]

    for r in res:
        print(r)

    print()

    # for line in lines:
    #     print(line)

print("Stations:")
extract_stations(svg_titles_path)
print("Transfers:")
extract_transfers(svg_chart_path)
print("Railways:")
extract_railways(svg_chart_path)

# for sub_dir in os.listdir(directory):
#     print(os.listdir(directory + '/' + sub_dir))