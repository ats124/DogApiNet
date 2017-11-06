DogApiNet - .NET client for Datadog's API
===

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

## Installation

Getting started from downloading NuGet packages.

```
PM> Install-Package DogApiNet -Version 1.0.0-alpha1
```

## Usage

```C#
using DogApiNet;
```

```C#
const string API_KEY = "api_key";
const string APP_KEY = "app_key";

async Task Main()
{
	using (var client = new DogApiClient(API_KEY, APP_KEY))
	{
		var end = DateTimeOffset.Now;
		var start = end.AddMinutes(-60);
		var events = await client.Event.QueryAsync(start, end, tags: new[] { "project:hogehoge", "stack:production"});
		events.Dump();
	}
}
```

## License

[MIT](LICENSE)
