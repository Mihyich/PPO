with open("measure.txt", 'r') as f:
    nomer = 0

    for line in f:
        nomer += 1

        size = float(line.split()[0])

        if (nomer % 50 == 0):
            print(nomer, size * 1.25)
