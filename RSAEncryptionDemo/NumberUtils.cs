//Chase Brower, 2023

using System.Numerics;
using System.Security.Cryptography;

namespace RSAEncryptionDemo;

public static class NumberUtils
{
    public static int PRIME_BIT_SIZE = 1024;
    public static int PSEUDOPRIME_ROUNDS = 400;
    private static readonly RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();

    public static BigInteger Encrypt(BigInteger numberToEncrypt, (BigInteger, BigInteger) publicKey)
    {
        return ModularExponentiate(numberToEncrypt, publicKey.Item2, publicKey.Item1);
    }
    public static BigInteger Decrypt(BigInteger numberToDecrypt, (BigInteger, BigInteger) privateKey)
    {
        return ModularExponentiate(numberToDecrypt, privateKey.Item2, privateKey.Item1);
    }

    public static BigInteger ModularExponentiate(BigInteger baseNum, BigInteger exponent, BigInteger modulus)
    {
        //Works via exponentiation by squaring
        if (modulus == 1) return 0;
        BigInteger result = 1;
        baseNum = baseNum % modulus;
        while(exponent > 0)
        {
            if(exponent % 2 == 1)
            {
                result = (result * baseNum) % modulus;
            }
            exponent = exponent >> 1;
            baseNum = (baseNum * baseNum) % modulus;
        }
        return result;
    }

    public static (BigInteger, BigInteger) ExtendedEuclideanAlgorithm(BigInteger n1, BigInteger n2)
    {
        (BigInteger, BigInteger) r = (n1, n2);
        (BigInteger, BigInteger) s = (1, 0);
        (BigInteger, BigInteger) t = (0, 1);

        BigInteger quotient;

        while(r.Item2 != 0)
        {
            quotient = r.Item1 / r.Item2;

            r = (r.Item2, r.Item1 - quotient * r.Item2);
            s = (s.Item2, s.Item1 - quotient * s.Item2);
            t = (t.Item2, t.Item1 - quotient * t.Item2);
        }

        //Bezout coefficients
        return (s.Item1, t.Item1);
    }

    public static BigInteger LeastCommonMultiple(BigInteger n1, BigInteger n2)
    {
        //lcm(a, b) = |ab| / gcd(a, b)
        return BigInteger.Abs(n1 * n2) / GreatestCommonDenominator(n1, n2);
    }

    public static BigInteger GreatestCommonDenominator(BigInteger n1, BigInteger n2)
    {
        //Euclid's algorithm
        while(n2 != 0)
        {
            BigInteger temp = n2;
            n2 = n1 % n2;
            n1 = temp;
        }

        return n1;
    }

    public static (BigInteger, BigInteger) GeneratePQPseudoPrimes()
    {
        //Generate primes
        BigInteger p = GeneratePseudoPrime();
        BigInteger q = GeneratePseudoPrime();

        //Return the prime tuple
        return (p, q);
    }

    private static BigInteger GeneratePseudoPrime() // Miller-Rabin primality test
    {
        //Get random value
        BigInteger bigInteger = GenerateRandomValue(PRIME_BIT_SIZE);

        //Make the number odd
        if (bigInteger % 2 == 0) bigInteger += 1;

        //Move up 2 at a time until a prime is found
        while (!IsPseudoPrime(bigInteger))
        {
            //Move up 2
            bigInteger += 2;
        }

        return bigInteger;
    }

    private static bool IsPseudoPrime(BigInteger bigInteger) // Miller-Rabin primality test
    {
        (BigInteger, BigInteger) sd = MillerRabinFactor(bigInteger);

        BigInteger y = 0;

        for (int i = 0; i < PSEUDOPRIME_ROUNDS; i++)
        {
            BigInteger a = (GenerateRandomValue(PRIME_BIT_SIZE) % (bigInteger - 4)) + 2;
            BigInteger x = ModularExponentiate(a, sd.Item2, bigInteger);

            for(int k = 0; k < sd.Item1; k++)
            {
                y = (x * x) % bigInteger;
                
                if(y == 1 && x != 1 && x != bigInteger - 1)
                {
                    return false;
                }
            }

            if(y != 1)
            {
                return false;
            }
        }

        return true;
    }

    private static (BigInteger, BigInteger) MillerRabinFactor(BigInteger n)
    {
        //Attempts to find (s, d) to satisfy n - 1 = (2^s) * d for the Miller-Rabin test
        BigInteger s = 0;
        BigInteger d = 0;

        BigInteger m = n - 1;
        while(m % 2 == 0)
        {
            s++;
            m /= 2;
        }

        d = m;

        return (s, d);
    }

    public static (BigInteger, BigInteger) GeneratePQPrimes()
    {
        //Generate primes
        BigInteger p = GeneratePrime();
        BigInteger q = GeneratePrime();

        //Return the prime tuple
        return (p, q);
    }

    private static BigInteger GeneratePrime() // aka sieve of eratosthenes
    {

        //Get random value
        BigInteger bigInteger = GenerateRandomValue(PRIME_BIT_SIZE);

        //Make the number odd
        if (bigInteger % 2 == 0) bigInteger += 1;

        //Move up 2 at a time until a prime is found
        while (!IsPrime(bigInteger))
        {
            //Move up 2
            bigInteger += 2;
        } 

        return bigInteger;
    }

    private static BigInteger GenerateRandomValue(int bitCount)
    {
        //Initialize local variables
        BigInteger bigInteger = 0;
        char[] randomBinaryString = new char[bitCount];
        byte[] randomBytes = new byte[bitCount / 8];

        //Generate random bytes
        randomNumberGenerator.GetBytes(randomBytes);

        //Transplate random bytes into binary string
        for (int i = 0; i < randomBytes.Length; i++)
        {
            char[] bits = ConvertToBinaryString(randomBytes[i]);
            for (int j = 0; j < 8; j++)
            {
                randomBinaryString[i * 8 + j] = bits[j];
            }
        }

        //Manufacture BigInteger from binary string
        for (int i = 0; i < PRIME_BIT_SIZE; i++)
        {
            bigInteger <<= 1;
            bigInteger += (randomBinaryString[i] == 1 ? 1 : 0);
        }

        return bigInteger;
    }

    private static char[] ConvertToBinaryString(byte b)
    {
        char[] bits = new char[8];
        int[] bl = new int[8];

        for (int i = 0; i < 8; i++)
        {
            bl[bl.Length - 1 - i] = ((b & (1 << i)) != 0) ? 1 : 0;
        }

        for(int i = 0; i < 8; i++)
        {
            bits[i] = (char)bl[i];
        }

        return bits;
    }

    public static bool IsPrime(BigInteger bigInteger)
    {
        if (bigInteger <= 1) return false;
        if (bigInteger == 2) return true;
        if (bigInteger % 2 == 0) return false;
        if (bigInteger % 5 == 0) return false;

        BigInteger upperBound = bigInteger.Sqrt();

        for(int i = 3; i <= upperBound; i+= 2)
        {
            if (bigInteger % i == 0) return false;
        }

        return true;
    }

    private static BigInteger Sqrt(this BigInteger n)
    {
        if (n == 0) return 0;
        if (n > 0)
        {
            int bitLength = Convert.ToInt32(Math.Ceiling(BigInteger.Log(n, 2)));
            BigInteger root = BigInteger.One << (bitLength / 2);

            while (!isSqrt(n, root))
            {
                root += n / root;
                root /= 2;
            }

            return root;
        }

        throw new ArithmeticException("NaN");
    }

    private static bool isSqrt(BigInteger n, BigInteger root)
    {
        BigInteger lowerBound = root * root;
        BigInteger upperBound = (root + 1) * (root + 1);

        return (n >= lowerBound && n < upperBound);
    }
}
