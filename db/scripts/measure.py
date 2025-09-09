import json
import psycopg2
from psycopg2.extras import RealDictCursor

DB_CONFIG = {
    'host': 'localhost',
    'database': 'metro_test',
    'user': 'postgres',
    'password': '1234',
    'port': 5432,
}

JSON_FILE_PATH = "cities/Moscow/chart.json"
BASE_CITY_NAME = "Москва"


def get_db_connection():
    return psycopg2.connect(**DB_CONFIG)

def modify_json(json_data, new_city_name: str) -> str: 
    if "city" not in json_data:
        raise KeyError(f'Поле "city" не найдено')
    
    json_data["city"] = new_city_name
    return json.dumps(json_data, ensure_ascii=False, indent=2)

def insert_chart(conn, json_text: str) -> tuple:
    with conn.cursor() as cur:
        cur.execute("SELECT * FROM add_chart_json_with_timing(%s)", (json_text,))
        chart_id, exec_time = cur.fetchone()
        conn.commit()
        return chart_id, exec_time
    
def measure_size() -> int:
    with conn.cursor() as cur:
        cur.execute("SELECT * FROM measure_size_mb()")
        size = cur.fetchone()[0]
        conn.commit()
        return size

if __name__ == "__main__":
    try:
        with open(JSON_FILE_PATH, 'r', encoding='utf-8') as f:
            json_data = json.load(f)

        conn = get_db_connection()
        print("Подключено к PostgreSQL.")

        with open('measure_idx.txt', 'w', encoding='utf-8') as f:

            for nomer in range(1, 1000 + 1):
                TARGET_CITY_NAME = BASE_CITY_NAME + "_" + str(nomer)
                modified_json = modify_json(json_data, TARGET_CITY_NAME)

                chart_id, time = insert_chart(conn, modified_json)
                size = measure_size()
                f.write(str(size) + ' ' + str(time) + '\n')

                if (nomer % 100 == 0):
                    print(str(nomer) + ': ' + str(size) + ' ' + str(time) + '\n')

    except Exception as e:
        print(f"Ошибка: {e}")
    finally:
        if 'conn' in locals():
            conn.close()
            print("Подключение закрыто.")