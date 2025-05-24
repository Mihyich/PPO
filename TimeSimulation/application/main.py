import math
from scipy.stats import norm, gamma

def calcStationEntryOrExitTime(
    occupancy: int,
    zeta=15,
    rho: float = 3,
    k: float = 0.5,
    gamma: float = 0.95
) -> float:
    """Расчет времени входа/выхода на/со станц(ию/ии)"""
    
    # Скорректированное среднее значение времени входа
    mu_base = zeta * (1 + k * occupancy / 10)

    # Преобразование матожидания и дисперсии, ожидаемые логнормальным распределением
    sigma_ln = math.sqrt(math.log(1 + (rho / mu_base) ** 2))
    mu_ln = math.log(mu_base) - 0.5 * sigma_ln ** 2

    # Квантиль
    z = norm.ppf(gamma)

    # Итоговое время входа
    return math.exp(mu_ln + z * sigma_ln)


def calcTrainOnRailwayTime(
    dur: float,
    acbsOcc: float = 6.5,
    absOcc: float = 5,
    rho: float = 3,
    k1: float = 0.25,
    k2: float = 0.5,
    gamma: float = 0.95
) -> float:
    """Расчет времени движения поезда по перегону"""
    
    # Скорректированное среднее значение времени на перегоне
    mu_base = dur * (1 + k1 * acbsOcc / 10 + k2 * absOcc / 10)

    # Преобразование матожидания и дисперсии, ожидаемые логнормальным распределением
    sigma_ln = math.sqrt(math.log(1 + (rho / mu_base) ** 2))
    mu_ln = math.log(mu_base) - 0.5 * sigma_ln ** 2

    # Квантиль
    z = norm.ppf(gamma)

    # Итоговое время входа
    return math.exp(mu_ln + z * sigma_ln)


def calcTrainWaitOnStationTime(
    atwT: float,
    dtwT: float,
    absOcc: float,
    occ: float,
    k1: float = 0.25,
    k2: float = 0.5,
    g: float = 0.95
) -> float:
    """Расчет времени ожидания поезда на станции"""
    
    # Скорректированное среднее значение времени на перегоне
    mu_base = atwT * (1 + k1 * absOcc / 10 + k2 * occ / 10)

    # Параметры гамма-распределения
    shape = (mu_base / dtwT)**2
    scale = dtwT**2 / mu_base

    # Квантиль
    return gamma.ppf(g, a=shape, scale=scale)


def main():
    occupancy = 10 # уровень загруженности станции
    dur = 3*60     # среднее время движения по перегону
    acbsOcc = 10   # средняя загруженность станций веток схемы
    absOcc = 10    # средняя загруженность станций ветки
    atwT = 60      # среднее время ожидания поезда на станции
    dtwT = 30      # разброс времени ожидания поезда на станции
    zeta = 3*60    # среднее время входа, c.
    rho = 10       # разброс времени входа, c.
    k = 0.5        # линейная значимость уровня загруженности станции
    k1 = 0.15      # линейная значимость уровня загруженности схемы
    k2 = 0.25      # линейная значимость уровня загруженности ветки
    gamma = 0.8    # доверительный уровень

    res = calcStationEntryOrExitTime(occupancy, zeta, rho, k, gamma)
    minutes, seconds = int(res // 60), int(res % 60)
    print(f"Пассажир потратит {minutes}:{seconds:02d} на вход на станцию")

    res = calcTrainOnRailwayTime(dur, acbsOcc, absOcc, rho, k1, k2, gamma)
    minutes, seconds = int(res // 60), int(res % 60)
    print(f"Поезд потратит {minutes}:{seconds:02d} на перегон")

    res = calcTrainWaitOnStationTime(atwT, dtwT, absOcc, occupancy, k1, k2, gamma)
    minutes, seconds = int(res // 60), int(res % 60)
    print(f"Поезд потратит {minutes}:{seconds:02d} на ожидание на станции")

if __name__ == "__main__":
    main()