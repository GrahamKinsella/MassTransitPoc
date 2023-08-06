using MassTransit;
using MassTransitPoc.Consumers;
using MassTransitPoc.Domain;
using MassTransitPoc.Producers;
using MassTransitPoc.UseCases.CreateBrand;
using MassTransitPoc.UseCases.CreateInvite;
using MassTransitPoc.UseCases.CreateUser;
using MassTransitPoc.UseCases.SendEmail;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMassTransit(x =>
{
    //need but not used
    x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context, KebabCaseEndpointNameFormatter.Instance));

    x.AddRider(rider =>
    {
        //configure producers
        rider.AddProducer<InviteUpdatedEvent>("internal-development-retailer-wizardinviteupdated");
        rider.AddProducer<InviteFailedEvent>("internal-development-retailer-wizardinvitefailed");

        //configure consumers
        rider.AddConsumer<InviteUpdatedEventConsumer>();
        rider.AddConsumer<InviteFailedEventConsumer>();

        rider.AddSagaStateMachine<InviteStateMachine, InviteState>(
                cfg => cfg.UseInMemoryOutbox()
            )
            .CosmosRepository("https://localhost:23456/",
                "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==", r =>
                {
                    r.DatabaseId = "test";
                    r.CollectionId = "sagas";
                });

        rider.UsingKafka((context, k) =>
        {
            //lower env config

            //k.Host("localhost:9092", h =>
            //{
            //    h.UseSasl(x =>
            //    {
            //        x.Username = "API Key"; Get from KV
            //        x.Password = "API secret"; Get from KV
            //        x.SecurityProtocol = SecurityProtocol.Plaintext;
            //    });
            //});

            k.Host("localhost:9092");

            //environment needs to be resolved

            k.TopicEndpoint<InviteUpdatedEvent>("internal-development-retailer-wizardinviteupdated", "development-wizard-internal-api",
                e =>
                {
                    e.CreateIfMissing();
                    e.ConfigureConsumer<InviteUpdatedEventConsumer>(context);
                    e.ConfigureSaga<InviteState>(context);
                });

            k.TopicEndpoint<InviteFailedEvent>("internal-development-retailer-wizardinvitefailed", "development-wizard-internal-api",
                e =>
                {
                    e.CreateIfMissing();
                    e.ConfigureConsumer<InviteFailedEventConsumer>(context);
                    e.ConfigureSaga<InviteState>(context);
                });
        });
    });
});

builder.Services.AddMediator(cfg =>
{
    cfg.AddConsumer<CreateInviteUseCase>();
    cfg.AddConsumer<InviteStateProducerUseCase>();
    cfg.AddConsumer<CreateBrandUseCase>();
    cfg.AddConsumer<CreateUserUseCase>();
    cfg.AddConsumer<SendEmailUseCase>();
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();