from datetime import datetime
import numpy as np
import matplotlib.pyplot as plt

FILE_MEASURE_WO_IDX = './measure.txt'
FILE_MEASURE_WITH_IDX = './measure_idx.txt'

TIME_RECORD_WO_IDX_X = []
TIME_RECORD_WO_IDX_Y = []

TIME_RECORD_WO_IDX_LINEAR_X = []
TIME_RECORD_WO_IDX_LINEAR_Y = []

TIME_RECORD_WITH_IDX_X = []
TIME_RECORD_WITH_IDX_Y = []

TIME_RECORD_WITH_IDX_LINEAR_X = []
TIME_RECORD_WITH_IDX_LINEAR_Y = []

SIZE_RECORD_WO_IDX_X = []
SIZE_RECORD_WO_IDX_Y = []

SIZE_RECORD_WO_IDX_LINEAR_X = []
SIZE_RECORD_WO_IDX_LINEAR_Y = []

SIZE_RECORD_WITH_IDX_X = []
SIZE_RECORD_WITH_IDX_Y = []

SIZE_RECORD_WITH_IDX_LINEAR_X = []
SIZE_RECORD_WITH_IDX_LINEAR_Y = []

with open(FILE_MEASURE_WO_IDX, 'r') as f1:
    record_nomer = 1
    for line in f1:
        data = line.split()
        time_str = data[1]
        time_obj = datetime.strptime(time_str, "%H:%M:%S.%f")
        midnight = time_obj.replace(hour=0, minute=0, second=0, microsecond=0)
        time_delta = time_obj - midnight
        time_milliseconds = (time_delta.total_seconds() * 1000)

        size_mb = float(data[0])

        TIME_RECORD_WO_IDX_X += [record_nomer]
        TIME_RECORD_WO_IDX_Y += [time_milliseconds]

        SIZE_RECORD_WO_IDX_X += [record_nomer]
        SIZE_RECORD_WO_IDX_Y += [size_mb]
        
        record_nomer += 1

with open(FILE_MEASURE_WITH_IDX, 'r') as f1:
    record_nomer = 1
    for line in f1:
        data = line.split()
        time_str = data[1]
        time_obj = datetime.strptime(time_str, "%H:%M:%S.%f")
        midnight = time_obj.replace(hour=0, minute=0, second=0, microsecond=0)
        time_delta = time_obj - midnight
        time_milliseconds = (time_delta.total_seconds() * 1000)

        size_mb = float(data[0])

        TIME_RECORD_WITH_IDX_X += [record_nomer]
        TIME_RECORD_WITH_IDX_Y += [time_milliseconds]

        SIZE_RECORD_WITH_IDX_X += [record_nomer]
        SIZE_RECORD_WITH_IDX_Y += [size_mb]
        
        record_nomer += 1

TIME_RECORD_WO_IDX_LINEAR_X = np.linspace(TIME_RECORD_WO_IDX_X[0], TIME_RECORD_WO_IDX_X[-1], 50)
TIME_RECORD_WO_IDX_LINEAR_Y = np.interp(TIME_RECORD_WO_IDX_LINEAR_X, TIME_RECORD_WO_IDX_X, TIME_RECORD_WO_IDX_Y)

TIME_RECORD_WITH_IDX_LINEAR_X = np.linspace(TIME_RECORD_WITH_IDX_X[0], TIME_RECORD_WITH_IDX_X[-1], 50)
TIME_RECORD_WITH_IDX_LINEAR_Y = np.interp(TIME_RECORD_WITH_IDX_LINEAR_X, TIME_RECORD_WITH_IDX_X, TIME_RECORD_WITH_IDX_Y)

# Первое окно: Время выполнения
plt.figure(figsize=(8, 5))  # ← Это создаёт ОТДЕЛЬНОЕ окно
plt.plot(TIME_RECORD_WO_IDX_LINEAR_X, TIME_RECORD_WO_IDX_LINEAR_Y, label='Без индекса')
plt.plot(TIME_RECORD_WITH_IDX_LINEAR_X, TIME_RECORD_WITH_IDX_LINEAR_Y, label='С индексом')
plt.title('График зависимости времени выполнения запроса от количества записей', fontsize=16)
plt.xlabel('Количество записей в таблице chart', fontsize=16)
plt.ylabel('Время (мс)', fontsize=16)
plt.legend(fontsize=16)
plt.grid(True)
# Не вызываем plt.show() пока — отложим до конца

# Второе окно: Размер данных
plt.figure(figsize=(8, 5))  # ← Это создаёт ВТОРОЕ отдельное окно
plt.plot(SIZE_RECORD_WO_IDX_X, SIZE_RECORD_WO_IDX_Y, label='Без индекса')
plt.plot(SIZE_RECORD_WITH_IDX_X, SIZE_RECORD_WITH_IDX_Y, label='С индексом')
plt.title('График зависимости объема дискового пространства от количества записей', fontsize=16)
plt.xlabel('Количество записей в таблице chart', fontsize=16)
plt.ylabel('Размер (МБ)', fontsize=16)
plt.legend(fontsize=16)
plt.grid(True)

# Теперь показываем ВСЕ созданные окна
plt.show()  # ← Покажет оба окна одновременно



