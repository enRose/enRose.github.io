private string GetQueryParameterBy(string token)
{
    string paramValue = null;

    if (string.Equals(token, QueryStringConstants.Thanos,
        StringComparison.OrdinalIgnoreCase))
    {
        paramValue = GetThanos();
    }
    else if (string.Equals(token, QueryStringConstants.SpiderMan,
        StringComparison.OrdinalIgnoreCase))
    {
        paramValue = GetSpiderMan();
    }
    else if (string.Equals(token, QueryStringConstants.IronMan,
        StringComparison.OrdinalIgnoreCase))
    {
        paramValue = GetIronMan();
    }
    else if (string.Equals(token, QueryStringConstants.CaptainAmerica,
        StringComparison.OrdinalIgnoreCase))
    {
        paramValue = GetCaptainAmerica();
    }
    else if (string.Equals(token, QueryStringConstants.WandaMaximoff,
        StringComparison.OrdinalIgnoreCase))
    {
        paramValue = GetWandaMaximoff();
    }
    else if (string.Equals(token, QueryStringConstants.Thor,
        StringComparison.OrdinalIgnoreCase))
    {
        paramValue = GetThor();
    }
    else if (string.Equals(token, QueryStringConstants.Hulk,
        StringComparison.OrdinalIgnoreCase))
    {
        paramValue = GetHulk();
    }
    else if (string.Equals(token, QueryStringConstants.Deadpool,
        StringComparison.OrdinalIgnoreCase))
    {
        paramValue = GetDeadpool();
    }

    return paramValue;
}
