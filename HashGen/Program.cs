var newHash = BCrypt.Net.BCrypt.HashPassword("Admin123!", workFactor: 11);
Console.WriteLine("New hash: " + newHash);
Console.WriteLine("Verify Admin123! => " + BCrypt.Net.BCrypt.Verify("Admin123!", newHash));
