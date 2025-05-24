import math
from scipy.stats import norm

def calc_station_entry_time(
    occupancy: int,
    zeta=15,
    rho: float = 3,
    k: float = 0.5,
    gamma: float = 0.95
) -> float:
    """Расчет времени входа на станцию"""
    
    # Скорректированное среднее значение времени входа
    mu_base = zeta * (1 + k * occupancy / 10)

    # Преобразование матожидания и дисперсии, ожидаемые логнормальным распределением
    mu_ln = math.log(mu_base) - 0.5 * math.log(1 + (rho / mu_base) ** 2)
    sigma_ln = math.sqrt(math.log(1 + (rho / mu_base) ** 2))

    # Квантиль
    z = norm.ppf(gamma)

    # Итоговое время входа
    return math.exp(mu_ln + z * sigma_ln)

def main():
    occupancy = 10 # уровень загруженности станции
    zeta = 3*60   # среднее время входа, c.
    rho = 60      # разброс времени входа, c.
    k = 0.5       # линейная значимость уровня загруженности станции
    gamma = 0.9   # доверительный уровень

    res = calc_station_entry_time(occupancy, zeta, rho, k, gamma)
    minutes, seconds = int(res // 60), int(res % 60)
    print(f"Пассажир потратит {minutes}:{seconds:02d} на вход на станцию")

if __name__ == "__main__":
    main()