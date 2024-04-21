using Zadanie6.Animals;

namespace Zadanie6.Repositories;

public interface IAnimalsRepository
{
    IEnumerable<Animal> GetAnimals();
    int CreateAnimal(Animal animal);
    int UpdateAnimal(Animal animal);
    int DeleteAnimal(int idAnimal);
}