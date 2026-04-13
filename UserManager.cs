using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class UserManager
{
    private static readonly string storageFile = "users.db";
    private static Dictionary<string, string> users = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    private static string rememberedUserKey = "__remembered__";

    public static void LoadUsers()
    {
        users.Clear();
        if (!File.Exists(storageFile)) return;

        foreach (var line in File.ReadAllLines(storageFile))
        {
            var parts = line.Split(':');
            if (parts.Length >= 2)
            {
                var key = parts[0];
                var val = parts[1];
                users[key] = val;
            }
        }
    }

    public static bool Register(string username, string password, bool remember)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)) return false;
        if (users.ContainsKey(username)) return false;

        var hash = Hash(password);
        users[username] = hash;
        if (remember) users[rememberedUserKey] = username;
        Save();
        return true;
    }

    public static bool Login(string username, string password, bool remember)
    {
        if (!users.ContainsKey(username)) return false;
        var expected = users[username];
        if (expected != Hash(password)) return false;
        if (remember) users[rememberedUserKey] = username; else if (users.ContainsKey(rememberedUserKey) && users[rememberedUserKey] == username) users.Remove(rememberedUserKey);
        Save();
        return true;
    }

    public static void Logout(string username)
    {
        if (users.ContainsKey(rememberedUserKey) && users[rememberedUserKey] == username)
        {
            users.Remove(rememberedUserKey);
            Save();
        }
    }

    public static string GetRememberedUser()
    {
        if (users.ContainsKey(rememberedUserKey)) return users[rememberedUserKey];
        return null;
    }

    private static void Save()
    {
        var lines = new List<string>();
        foreach (var kv in users)
        {
            lines.Add(kv.Key + ":" + kv.Value);
        }
        File.WriteAllLines(storageFile, lines);
    }

    private static string Hash(string input)
    {
        using (var sha = SHA256.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
