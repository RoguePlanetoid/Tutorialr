using Spotify.NetStandard.Client.Authentication;
using Spotify.NetStandard.Client.Authentication.Enums;
using System.Text.Json;

public class Token
{
    public AccessToken AccessToken { get; set; }
    public bool HasUserToken => AccessToken?.TokenType == TokenType.User;
    public bool HasToken => AccessToken?.TokenType == TokenType.Access || HasUserToken;

    public Token(AccessToken token) => AccessToken = token;

    public Token(string content) => AccessToken = JsonSerializer.Deserialize<AccessToken>(content);

    public override string ToString() => JsonSerializer.Serialize(AccessToken);
}