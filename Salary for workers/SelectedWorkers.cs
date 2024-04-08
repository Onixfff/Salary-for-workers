public class SelectedWorkers
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Surname { get; private set; }
    public string Patronymic { get; private set; }

    public SelectedWorkers( int id, string name, string surname, string patronymic)
    {
        Id = id;
        Name = name;
        Surname = surname;
        Patronymic = patronymic;
    }
}
