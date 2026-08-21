using System;

class BankAccount
{
    double balance;

    public BankAccount(double balance)
    {
        this.balance = balance;
    }

    public double Withdraw(double amount)
    {
        if (amount < 0)
            throw new ArgumentException();
        if (amount > balance)
            throw new InvalidOperationException();
        balance -= amount;
        return balance;
    }
}

class Problem10
{
    static void Main()
    {
        BankAccount account = new BankAccount(1000);
        try
        {
            Console.Write("Enter withdrawal amount: ");
            double amount = double.Parse(Console.ReadLine());
            Console.WriteLine($"Withdrawal successful, new balance: {account.Withdraw(amount)}");
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine("Insufficient balance!");
        }
        catch (ArgumentException)
        {
            Console.WriteLine("Invalid amount!");
        }
        catch (FormatException)
        {
            Console.WriteLine("Please enter a valid amount");
        }
    }
}
