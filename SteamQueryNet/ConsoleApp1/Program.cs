// See https://aka.ms/new-console-template for more information
using SteamQueryNet48;
using SteamQueryNet48.Interfaces;

string text = Console.ReadLine();

//46.174.51.106:40000

IServerQuery serverQuery = new ServerQuery();
var info = serverQuery.Connect(text).GetRulesAsync().Result;

foreach (var rule in info)
    Console.WriteLine(rule.Name + ": " + rule.Value);

Console.ReadLine();


