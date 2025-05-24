# Математическое обоснование симуляции времени маршрута

## <a id="Model">1. Описание модели</a>

### <a id="Entities">1.1 Сущности</a>

Раcсматриваемая схема метро состоит из ключевых сущностей:

1. **Станция** - S,
2. **Переезд** - R,
3. **Переход** - T.

Ветки входят в вычисления косвенно, достаточно рассмотреть сущности выше.

### <a id="LinkTypes">1.2 Типы связей</a>

Вычисления будут происходит между [сущностями](#Entities) определенным образом:

| Связь                 | _Станция_, (***S2***) | _Переезд_, (***R2***) | _Переход_ (***T2***) |
| --------------------- | --------------------- | --------------------- | -------------------- |
| _Станция_, (***S1***) | :x:                   | :white_check_mark:    | :white_check_mark:   |
| _Переезд_, (***R1***) | :white_check_mark:    | :x:                   | :x:                  |
| _Переход_, (***T1***) | :white_check_mark:    | :x:                   | :x:                  |

То есть, необходимо обработать четыре вида связей:

1. **S** &#8594; **R**,
2. **R** &#8594; **S**,
3. **S** &#8594; **T**,
4. **T** &#8594; **S**.

## <a id="Params">2. Параметры</a>

### <a id="LocalParams">2.1 Локальные параметры</a>

Локальные параметры каждой из [сущностей](#Entities) заданы на этапе их создания. Они атомарны, то есть не могут изменять в рамках одной симуляции времени всего маршрута.

Существует несколько параметров:

1. **Станция**:
    - ___Occupancy___ - уровень загруженности станции;
2. **Переезд**:
    - ___Duration___ - среднее время движения поезда по перегону;
3. **Переход**:
    - ___Occupany___ - уровень загруженности перехода;
    - ___Duration___ - среднее время движение пассажира по переходу, при нулевой загруженности.

### <a id="LocalParamsConstraints">Ограничения локальных параметров</a>

| Имя             | Описание                                                           | Тип                          | Ограничения           |
| ----------------| ------------------------------------------------------------------ | ---------------------------- | --------------------- |
| ___Occupancy___ | Уровень загруженности, выраженный в виде оценки по x-бальной шкале | Целое, неотрицательное число | [0, 10]               |
| ___Duration___  | Временной отрезок времени, c.                                      | Целое, неотрицательное число | [0, +&infin;]         |

### <a id="GlobaParams">2.2 Глобальные параметры</a>

Глобальные параметры подразделяются на категории:

1. **Произвольные константы**:
    - ___AverStationEntryTime___ - среднее время входа пассажира на станцию;
    - ___AverStationExitTime___ - среднее время выхода пассажира со станции;
    - ___AverTrainWaitTime___ - среднее время ожидание поезда на станции;
    - ___QuintileTrustLevel___ - уровень доверия квантилей;
2. **Характеристические данные**:
    - ___AverChartBranchesStationsOccupancy___ - средняя загруженность станций веток схемы;
    - ___AverChartBranchesRailwaysDuration___ - среднее время движения поезда в перегонах веток схемы;
    - ___AverBranchStationsOccupancy___ - средняя загруженность станций ветки;
    - ___AverBranchRailwaysDuration___ - среднее время движения поезда в перегонах ветки.

### <a id="LocalParamsConstraints">Ограничения глобальных параметров</a>

1. **Произвольные константы**:

| Имя                        | Описание                                                               | Тип                              | Ограничения   |
| -------------------------- | ---------------------------------------------------------------------- | -------------------------------- | ------------- |
| ___AverStationEntryTime___ | Среднеарифметический временной отрезок входа пассажира на станцию, c.  | Целое, неотрицательное число     | [0, +&infin;] |
| ___AverStationExitTime___  | Среднеарифметический временной отрезок выхода пассажира со станции, c. | Целое, неотрицательное число     | [0, +&infin;] |
| ___AverTrainWaitTime___    | Среднеарифметический временной отрезок ожидания поезда на станции, c.  | Целое, неотрицательное число     | [0, +&infin;] |
| ___QuintileTrustLevel___   | Уровень доверия квантилей, используемых в расчетах распределений       | Плавающее, неотрицательное число | [0, 1]        |

2. **Характеристические данные**:

| Имя                                      | Описание                                                                           | Тип                          | Ограничения   |
| ---------------------------------------- | ---------------------------------------------------------------------------------- | ---------------------------- | ------------- |
| ___AverChartBranchesStationsOccupancy___ | Среднеарифметический временной отрезок загруженности станций веток схемы, c.       | Целое, неотрицательное число | [0, +&infin;] |
| ___AverChartBranchesRailwaysDuration___  | Среднеарифметический временной отрезок движения поезда в перегонах веток схемы, c. | Целое, неотрицательное число | [0, +&infin;] |
| ___AverBranchStationsOccupancy___        | Среднеарифметический временной отрезок загруженности станций ветки, c.             | Целое, неотрицательное число | [0, +&infin;] |
| ___AverBranchRailwaysDuration___         | Среднеарифметический временной отрезок движения поезда в перегонах ветки, с.       | Целое, неотрицательное число | [0, +&infin;] |

## <a id="Justification">3. Вычисления</a>

### <a id="JustyLocalParams">3.1 Локальные параметры</a>

Пусть:

- **S<sub>i</sub>** - _i_-ая станция, где
<math display="inline">
  <mi>i</mi>
  <mo>=</mo>
  <mi>1</mi>
  <mo>,</mo>
  <mo>...</mo>
  <mo>,</mo>
  <msub>
    <mi>S</mi>
    <mi>N</mi>
  </msub>
</math> и
<math display="inline">
  <msub>
    <mi>S</mi>
    <mi>N</mi>
  </msub>
</math> - количество станций,

- **R<sub>j</sub>** - _j_-ый переезд, где
<math display="inline">
  <mi>j</mi>
  <mo>=</mo>
  <mi>1</mi>
  <mo>,</mo>
  <mo>...</mo>
  <mo>,</mo>
  <msub>
    <mi>R</mi>
    <mi>N</mi>
  </msub>
</math> и
<math display="inline">
  <msub>
    <mi>R</mi>
    <mi>N</mi>
  </msub>
</math> - количество переездов,

- **T<sub>k</sub>** - _k_-ый переход, где
<math display="inline">
  <mi>k</mi>
  <mo>=</mo>
  <mi>1</mi>
  <mo>,</mo>
  <mo>...</mo>
  <mo>,</mo>
  <msub>
    <mi>T</mi>
    <mi>N</mi>
  </msub>
</math> и
<math display="inline">
  <msub>
    <mi>T</mi>
    <mi>N</mi>
  </msub>
</math> - количетсво переходов.

Тогда:

- **Socc<sub>i</sub>** - загруженность _i_-ой станции,

- **Rdur<sub>j</sub>** - среднее время движения поезда по _j_-ому перегону,

- **Tocc<sub>k</sub>** - загруженность _k_-ого перехода,

- **Tdur<sub>k</sub>** - среднее время движения пассажира по _k_-ому переходу.

Все эти данные берутся из заготовленных таблиц.

### <a id="JustyGlobalParams">3.2 Глобальные параметры</a>

1. **Произвольные константы**:

    - ___AverStationEntryTime___, "**asEntryT**", <math display="inline"><mi>ζ</mi></math> - задать вручную;

    - ___AverStationExitTime___, "**asExitT**", <math display="inline"><mi>η</mi></math> - задать вручную;

    - ___AverTrainWaitTime___, "**atwaitT**", <math display="inline"><mi>τ</mi></math> - задать вручную;

    - ___QuintileTrustLevel___, "**qtl**", <math display="inline"><mi>γ</mi></math> - задать вручную;

2. **Характеристические данные**:

    - ___AverChartBranchesStationsOccupancy___, "_acbsOcc_", <math display="inline"><mi>Ο</mi></math> :
    <math display="block">
      <mi>Ο</mi>
      <mo>=</mo>
       <mfrac>
        <mn>1</mn>
        <msub>
          <mi>R</mi>
          <mi>N</mi>
        </msub>
      </mfrac>
      <munderover>
        <mo>∑</mo>
        <mrow>
          <mi>i</mi>
          <mo>=</mo>
          <mn>1</mn>
          </mrow>
        <msub>
          <mi>S</mi>
          <mi>N</mi>
        </msub>
      </munderover>
    <msub>
      <mi>Socc</mi>
      <mi>i</mi>
    </msub>
    <mtext>;</mtext>
    </math>

    - ___AverChartBranchesRailwaysDuration___, "**acbrDur**", <math display="inline"><mi>Υ</mi></math> :
    <math display="block">
      <mi>Υ</mi>
      <mo>=</mo>
      <mfrac>
        <mn>1</mn>
        <msub>
          <mi>R</mi>
          <mi>N</mi>
        </msub>
      </mfrac>
      <munderover>
        <mo>∑</mo>
        <mrow>
          <mi>j</mi>
          <mo>=</mo>
          <mn>1</mn>
          </mrow>
        <msub>
          <mi>R</mi>
          <mi>N</mi>
        </msub>
      </munderover>
    <msub>
      <mi>Rdur</mi>
      <mi>j</mi>
    </msub>
    <mtext>;</mtext>
    </math>

    - ___AverBranchStationsOccupancy___, "**absOcc**", <math display="inline"><msub><mi>ο</mi><mi>b</mi></msub></math> :
    <math display="block">
      <msub>
        <mi>ο</mi>
        <mi>b</mi>
    </msub>
      <mo>=</mo>
      <mfrac>
        <mn>1</mn>
        <msub>
          <mi>N</mi>
          <mi>b</mi>
        </msub>
      </mfrac>
      <munderover>
        <mo>∑</mo>
        <mrow>
          <mi>i</mi>
          </mrow>
        <msub>
          <mi>N</mi>
          <mi>b</mi>
        </msub>
      </munderover>
    <msub>
      <mi>Socc</mi>
      <mi>i</mi>
    </msub>
    <mtext>,</mtext>
    </math> для станций, которые принадлежат _b_-ой ветке;

    - ___AverBranchRailwaysDuration___, "**abrDur**", <math display="inline"><msub><mi>υ</mi><mi>b</mi></msub></math> :
    <math display="block">
      <msub>
        <mi>υ</mi>
        <mi>b</mi>
    </msub>
      <mo>=</mo>
      <mfrac>
        <mn>1</mn>
        <msub>
          <mi>N</mi>
          <mi>b</mi>
        </msub>
      </mfrac>
      <munderover>
        <mo>∑</mo>
        <mrow>
          <mi>j</mi>
          </mrow>
        <msub>
          <mi>N</mi>
          <mi>b</mi>
        </msub>
      </munderover>
    <msub>
      <mi>Rdur</mi>
      <mi>j</mi>
    </msub>
    <mtext>,</mtext>
    </math> для переездов, принадлежащих _b_-ой ветке.

