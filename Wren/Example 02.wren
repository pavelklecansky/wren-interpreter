var n = 27
while (n != 1) {
  System.print(n)
  if (n % 2 == 0) {
    n = n / 2
  } else {
    n = 3 * n + 1
  }
}
System.print("Konec")