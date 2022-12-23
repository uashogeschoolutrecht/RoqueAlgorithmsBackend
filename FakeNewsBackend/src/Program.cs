// See https://aka.ms/new-console-template for more information
using FakeNewsBackend;

var runner = new Runner();
runner.Setup();

await runner.Run();