using System.Threading.Tasks;
using TrueLayerChallenge.Domain.QueryModels;

namespace TrueLayerChallenge.Domain.Database
{
    public interface IPokemonDatastore
    {
        Task<Pokemon> GetPokemonAsync(string name);
        Task WritePokemonAsync(Pokemon pokemon);
    }
}
