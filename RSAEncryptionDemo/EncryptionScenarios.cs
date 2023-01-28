//Chase Brower, 2023

using System.Numerics;

namespace RSAEncryptionDemo;
public static class EncryptionScenarios
{
    public static void RunRSAPreciseScenario()
    {
        NumberUtils.PRIME_BIT_SIZE = 32;

        Console.WriteLine("Running RSA example with definite primes");

        //Select two large primes, p, and q

        (BigInteger, BigInteger) pq = NumberUtils.GeneratePQPrimes();

        Console.WriteLine($"{NumberUtils.PRIME_BIT_SIZE} bit prime pair: {pq}");

        BigInteger n = pq.Item1 * pq.Item2;

        Console.WriteLine($"Key length (rounded up to nearest byte): {n.ToByteArray().Length * 8} bits");

        Console.WriteLine($"PQ-Product: {n}");

        //Calculate lambda(n) under lambda -> carmichael's totient function

        BigInteger lambdaN = NumberUtils.LeastCommonMultiple(pq.Item1 - 1, pq.Item2 - 1);

        Console.WriteLine($"Carmichael's totient (lambda(n)): {lambdaN}");

        //Choose e, an integer coprime with carmichael's totient
        int e = 65537; // 2^16 + 1 -- this is prime and therefore coprime with carmichael's totient, and also fast and secure

        //Find d in the equation d*e = 1 (mod lambda(n)) via the extended euclidean algorithm
        (BigInteger, BigInteger) bezoutCoefficients = NumberUtils.ExtendedEuclideanAlgorithm(lambdaN, e);

        Console.WriteLine($"Bezout Coefficients: {bezoutCoefficients}");

        BigInteger d = lambdaN + bezoutCoefficients.Item2;

        Console.WriteLine($"d: {d}");

        //Print public and private key
        (BigInteger, BigInteger) publicKey = (n, e);
        (BigInteger, BigInteger) privateKey = (n, d);

        Console.WriteLine($"Public key: {publicKey}");
        Console.WriteLine($"Private key: {privateKey}");

        BigInteger message = 65;

        Console.WriteLine($"Example message: {message}");

        BigInteger encrypted = NumberUtils.Encrypt(message, publicKey);

        Console.WriteLine($"Encrypted form: {encrypted}");

        BigInteger decrypted = NumberUtils.Decrypt(encrypted, privateKey);

        Console.WriteLine($"Decrypted form: {decrypted}");
    }

    public static void RunRSAProbabilisticScenario()
    {
        NumberUtils.PRIME_BIT_SIZE = 1024;

        Console.WriteLine($"Running RSA example with probabilistic primes ({NumberUtils.PSEUDOPRIME_ROUNDS} rounds of Miller-Rabin)");

        //Select two large primes, p, and q

        (BigInteger, BigInteger) pq = NumberUtils.GeneratePQPseudoPrimes();

        Console.WriteLine($"{NumberUtils.PRIME_BIT_SIZE} bit prime pair: {pq}");

        BigInteger n = pq.Item1 * pq.Item2;

        Console.WriteLine($"Key length (rounded up to nearest byte): {n.ToByteArray().Length * 8} bits");

        Console.WriteLine($"PQ-Product: {n}");

        //Calculate lambda(n) under lambda -> carmichael's totient function

        BigInteger lambdaN = NumberUtils.LeastCommonMultiple(pq.Item1 - 1, pq.Item2 - 1);

        Console.WriteLine($"Carmichael's totient (lambda(n)): {lambdaN}");

        //Choose e, an integer coprime with carmichael's totient
        int e = 65537; // 2^16 + 1 -- this is prime and therefore coprime with carmichael's totient, and also fast and secure

        //Find d in the equation d*e = 1 (mod lambda(n)) via the extended euclidean algorithm
        (BigInteger, BigInteger) bezoutCoefficients = NumberUtils.ExtendedEuclideanAlgorithm(lambdaN, e);

        Console.WriteLine($"Bezout Coefficients: {bezoutCoefficients}");

        BigInteger d = lambdaN + bezoutCoefficients.Item2;

        Console.WriteLine($"d: {d}");

        //Print public and private key
        (BigInteger, BigInteger) publicKey = (n, e);
        (BigInteger, BigInteger) privateKey = (n, d);

        Console.WriteLine($"Public key: {publicKey}");
        Console.WriteLine($"Private key: {privateKey}");

        BigInteger message = 2147000000;

        Console.WriteLine($"Example message: {message}");

        BigInteger encrypted = NumberUtils.Encrypt(message, publicKey);

        Console.WriteLine($"Encrypted form: {encrypted}");

        BigInteger decrypted = NumberUtils.Decrypt(encrypted, privateKey);

        Console.WriteLine($"Decrypted form: {decrypted}");
    }
}