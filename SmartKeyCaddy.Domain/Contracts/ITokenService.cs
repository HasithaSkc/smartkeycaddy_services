﻿
using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Domain.Contracts;

public interface ITokenService
{
    ApiTokenResponse GetToken(Guid userId);
}
