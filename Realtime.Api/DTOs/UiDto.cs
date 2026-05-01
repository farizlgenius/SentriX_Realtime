using System;

namespace Realtime.Api.DTOs;

public sealed record UiDto(string Key,object? Data = default!);