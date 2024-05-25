### Домашнее задание №2. Добавляем EF Core с базой данных на SQLite и CRUD-API для основных сущностей.

#### 1. Добавить Entity Framework Core в DataAccess проект и сделать реализацию IRepository в виде EfRepository,
####    которая будет работать с DataContext Entity Framework.

Добавил Entity Framework Core в DataAccess проект.

Реализация EfRepository:
```cs
public class EfRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly DataBaseContext _datacontext;
    protected DbSet<T> Data;

    public EfRepository(DataBaseContext datacontext)
    {
        _datacontext = datacontext;
        Data = _datacontext.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await Data.ToListAsync();
    }

    public async Task<T> GetByIdAsync(Guid id)
    {
        return await Data.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<T>> GetRangeByIdsAsync(List<Guid> ids)
    {
        return await Data.Where(e => ids.Contains(e.Id)).ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await Data.AddAsync(entity);
        await _datacontext.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        Data.Update(entity);
        await _datacontext.SaveChangesAsync();
            
    }

    public async Task DeleteAsync(Guid Id)
    {
        var entity = await GetByIdAsync(Id);

        Data.Remove(entity);
        await _datacontext.SaveChangesAsync();           
    }

    public async Task<T> DeleteRangeAsync(IEnumerable<Guid> Ids)
    {
        var entities = await Data.FindAsync(Ids);
        Data.RemoveRange(entities);

        await _datacontext.SaveChangesAsync();
        return entities;
    }

    public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        return await Data.FirstOrDefaultAsync(predicate);
    }
}
```


#### 2. Добавить SQLite в качестве БД

Добавил = Startup.cs:
```cs
services.AddDbContext<DataBaseContext>(
            options =>
            {
                options.UseSqlite(Configuration.GetConnectionString("SQLiteConnectionString"));
            });
```

#### 3. База должна удаляться и создаваться каждый раз, заполняясь тестовыми данными из FakeDataFactory.

Заполнение тестовыми данными происходит внутри ...Configuration():

```cs
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.ApplyConfiguration(new RoleConfiguration());
    modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
    modelBuilder.ApplyConfiguration(new CustomerConfiguration());
    modelBuilder.ApplyConfiguration(new PreferenceConfiguration());
    modelBuilder.ApplyConfiguration(new CustomerPreferenceConfiguration());
    modelBuilder.ApplyConfiguration(new PromoCodeConfiguration());
}
```

Удаляется каждый раз здесь:
```cs
_context.Database.EnsureDeleted();
```

#### 4. Настроить маппинг классов Employee, Roles, Customer,Preference и PromoCode на базу данных через EF.Обратить внимание, что PromoCode имеет ссылку на Preference и Employee имеет ссылку на Role. Customer имеет набор Preference, но так как Preference - это общий справочник и сущности связаны через Many-to-many,то нужно сделать маппинг через сущность CustomerPreference. Строковые поля должны иметь ограничения на MaxLength. Связь Customer и Promocode реализовать через One-To-Many, будем считать, что в данном примере промокод может быть выдан только одному клиенту из базы.

Классы такие:
```cs
public class Employee
        : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public string FullName => $"{FirstName} {LastName}";

    public string Email { get; set; }
    public string Phone { get; set; }
    public Guid RoleId { get; set; }
    public virtual Role Role { get; set; }

    public int AppliedPromocodesCount { get; set; }
}

public class Role
        : BaseEntity
{
    public string Name { get; set; }

    public string Description { get; set; }

    public virtual ICollection<Employee> Employees { get; set; }
}

public class Customer
        :BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public string FullName => $"{FirstName} {LastName}";

    public string Email { get; set; }

    public ICollection<CustomerPreference> Preferences { get; set; }
    public ICollection<PromoCode> PromoCodes { get; set; }
    //TODO: Списки Preferences и Promocodes 
}

public class CustomerPreference
{
    public Guid CustomerId { get; set; }
    public virtual Customer Customer { get; set; }

    public Guid PreferenceId { get; set; }
    public virtual Preference Preference { get; set; }
}

public class Preference
        :BaseEntity
{
    public string Name { get; set; }

    public virtual ICollection<PromoCode> PromoCodes { get; set; }

    public virtual ICollection<CustomerPreference> Customers { get; set; }
}

public class PromoCode
        : BaseEntity
{
    public string Code { get; set; }

    public string ServiceInfo { get; set; }

    public DateTime BeginDate { get; set; }

    public DateTime EndDate { get; set; }

    public string PartnerName { get; set; }

    public Guid PreferenceId { get; set; }
    public Preference Preference { get; set; }

    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; }
}
```

#### 5. Реализовать CRUD операции для CustomersController через репозиторий EF, нужно добавить новый Generic класс EfRepository. Получение списка, получение одного клиента, создание/редактирование и удаление, при удалении также нужно удалить ранее выданные промокоды клиента. Методы должны иметь xml комментарии для описания в Swagger. CustomerResponse также должен возвращать список предпочтений клиента с той же моделью PrefernceResponse

CRUD операции:

Получение списка всех клиентов

```cs
[HttpGet]
public async Task<ActionResult<List<CustomerShortResponse>>> GetCustomersAsync()
{
    //TODO: Добавить получение списка клиентов
    var customers = await customerRepository.GetAllAsync();

    var customerModelList = customers.Select(c =>
        new CustomerShortResponse()
        {
            Id = c.Id,
            FirstName = c.FirstName,
            LastName = c.LastName,
            Email = c.Email
        }).ToList();

    return customerModelList;
}
```

Получение клиента вместе с выданными ему промомкодами

```cs
[HttpGet("{id}")]
public async Task<ActionResult<CustomerResponse>> GetCustomerAsync(Guid id)
{
    //TODO: Добавить получение клиента вместе с выданными ему промомкодами
    var customer = await customerRepository.GetByIdAsync(id);
    if (customer == null) return NotFound();
    var promoCodes = customer.PromoCodes;



    var customerModel = new CustomerResponse()
    {
        Id = customer.Id,
        FirstName = customer.FirstName,
        LastName = customer.LastName,
        Email = customer.Email,
        PromoCodes = promoCodes != null ? customer.PromoCodes.Select(p =>
            new PromoCodeShortResponse
            {
                Id = p.Id,
                Code = p.Code,
                ServiceInfo = p.ServiceInfo,
                BeginDate = p.BeginDate.ToString("yyyy.MM.dd"),
                EndDate = p.EndDate.ToString("yyyy.MM.dd"),
                PartnerName = p.PartnerName
            }).ToList() : null
    };

    return Ok(customerModel);
}
```


Добавление нового клиента вместе с его предпочтениями:

```cs
[HttpPost]
public async Task<IActionResult> CreateCustomerAsync(CreateOrEditCustomerRequest request)
{
    //TODO: Добавить создание нового клиента вместе с его предпочтениями
    var preferences = await preferenceRepository.GetRangeByIdsAsync(request.PreferenceIds);

    var customer = new Customer()
    {
        FirstName = request.FirstName,
        LastName = request.LastName,
        Email = request.Email,
    };
    customer.Preferences = preferences.Select(p => new CustomerPreference()
    {
        Customer = customer,
        Preference = p
    }).ToList();

    await customerRepository.AddAsync(customer);

    return Ok();            
}
```

Редактирование данных клиента вместе с его предпочтениями
```cs
[HttpPut("{id}")]
public async Task<IActionResult> EditCustomersAsync(Guid id, CreateOrEditCustomerRequest request)
{
    //TODO: Обновить данные клиента вместе с его предпочтениями
    var customer = await customerRepository.GetByIdAsync(id);

    if (customer == null)
        return NotFound();

    var preferences = await preferenceRepository.GetRangeByIdsAsync(request.PreferenceIds);

    customer.FirstName = request.FirstName;
    customer.LastName = request.LastName;
    customer.Email = request.Email;
    //customer.Preferences.Clear();
    customer.Preferences = preferences.Select(p => new CustomerPreference()
    {
        Customer = customer,
        Preference = p
    }).ToList();

    await customerRepository.UpdateAsync(customer);

    return Ok();
}
```

Удаление клиента вместе с выданными ему промокодами
```cs
[HttpDelete]
public async Task<IActionResult> DeleteCustomer(Guid id)
{
    //TODO: Удаление клиента вместе с выданными ему промокодами
    var customer = await customerRepository.GetByIdAsync(id);
    if (customer == null)
        return NotFound();

    var promocodes = customer.PromoCodes;

    if (promocodes != null)
    {
        await promocodesRepository.DeleteRangeAsync(promocodes.Select(p => p.Id));
    }

    await customerRepository.DeleteAsync(id);

    return Ok();
}
```





#### 6. Нужно реализовать контроллер, который возвращает список предпочтений (Preference) в виде PrefernceResponse модели из базы данных. Метод должен иметь xml комментарии для описания в Swagger.
```cs
[Route("api/v1/[controller]")]
[ApiController]
public class PreferenceController : ControllerBase
{
    private readonly IRepository<Preference> preferenceRepository;

    public PreferenceController(IRepository<Preference> preferenceRepository)
    {
        this.preferenceRepository = preferenceRepository;
    }

    /// <summary>
    /// Получить список всех предпочтений
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IEnumerable<PreferenceResponse>> GetPreferenceAsync()
    {
        var preference = await preferenceRepository.GetAllAsync();
        return preference.Select(x => new PreferenceResponse
        {
            Id = x.Id,
            Name = x.Name
        });
    }        
}
```

#### 7. В качестве дополнительного задания реализовать методы PromoCodesController. Метод GivePromocodesToCustomersWithPreferenceAsync должен сохранять новый промокод в базе данных и находить клиентов с переданным предпочтением и добавлять им данный промокод. GetPromocodesAsync - здесь даты передаются строками, чтобы не было проблем с часовыми поясами
```cs
/// <summary>
/// Промокоды
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class PromocodesController
    : ControllerBase
{
    public readonly IRepository<PromoCode> promocodesRepository;
    private readonly IRepository<Preference> preferenceRepository;
    private readonly IRepository<Customer> customerRepository;
    public PromocodesController(IRepository<PromoCode> promocodesRepository, IRepository<Preference> preferenceRepository, IRepository<Customer> customerRepository)
    {
        this.promocodesRepository = promocodesRepository;
        this.preferenceRepository = preferenceRepository;
        this.customerRepository = customerRepository;
    }
    /// <summary>
    /// Получить все промокоды
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<List<PromoCodeShortResponse>>> GetPromocodesAsync()
    {
        //TODO: Получить все промокоды 

        var promocodes = await promocodesRepository.GetAllAsync();

        var promocodeModelList = promocodes.Select(p =>
            new PromoCodeShortResponse()
            {
                Id = p.Id,
                Code = p.Code,
                ServiceInfo = p.ServiceInfo,
                BeginDate = p.BeginDate.ToString("yyyy.MM.dd"),
                EndDate = p.EndDate.ToString("yyyy.MM.dd"),
                PartnerName = p.PartnerName
            }).ToList();

        return promocodeModelList;
    }

    /// <summary>
    /// Создать промокод и выдать его клиентам с указанным предпочтением
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request)
    {
        var preference = await preferenceRepository
            .FirstOrDefaultAsync(p => p.Name == request.Preference);

        if (preference == null)
            return BadRequest();

        var customer = await customerRepository
            .FirstOrDefaultAsync(c => c.Preferences
            .Any(cp => cp.PreferenceId == preference.Id));

        if (customer == null)
            return NotFound($"Нет пользователей с предпочтением \"{request.Preference}\"");
        //TODO: Создать промокод и выдать его клиентам с указанным предпочтением
        PromoCode promoCode = new PromoCode
        {
            ServiceInfo = request.ServiceInfo,
            PartnerName = request.PartnerName,
            Code = request.PromoCode,
            Customer = customer,
            Preference = preference,
            BeginDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(14)
        };

        await promocodesRepository.AddAsync(promoCode);
        return Ok();
    }
}

```

#### 8. В качестве дополнительного задания реализовать две миграции: начальную миграцию и миграцию с изменением любого поля на выбор, в этом случае удаление на каждый запуск уже не должно происходить

Добавил начальную миграцию Initial:
```cmd
dotnet ef migrations add Initial --startup-project ../Otus.Teaching.PromoCodeFactory.WebHost/
```

Добавил номер телефона в Eployee и сделал соответсвующую миграцию миграцию EmployeePhone:
```cmd
dotnet ef migrations add EmployeePhone --startup-project ../Otus.Teaching.PromoCodeFactory.WebHost/
```

