using Zadanie6.Animals;
using Zadanie6.Repositories;

namespace Zadanie6.Services;

public class AnimalsService : IAnimalsService
{
    private readonly IAnimalsRepository _animalsRepository;

    public AnimalsService(IAnimalsRepository animalsRepository)
    {
        _animalsRepository = animalsRepository;
    }


    public IEnumerable<Animal> GetAnimals(string orderBy)
    {
        return _animalsRepository.GetAnimals(orderBy);
    }

    public int CreateAnimal(Animal animal)
    {
        return _animalsRepository.CreateAnimal(animal);
    }

    public int UpdateAnimal(Animal animal)
    {
        var existingAnimal = _animalsRepository.GetAnimalById(animal.IdAnimal);
        if (existingAnimal == null)
        {
            throw new Exception("Animal with the given id does not exist.");
        }
        
        return _animalsRepository.UpdateAnimal(animal);
    }

    public int DeleteAnimal(int idAnimal)
    {
        var existingAnimal = _animalsRepository.GetAnimalById(idAnimal);
        if (existingAnimal == null)
        {
            throw new Exception("Animal with the given id does not exist.");
        }
        return _animalsRepository.DeleteAnimal(idAnimal);
    }

}