using Confluent.Kafka;
using MassTransit;
using MassTransit.KafkaIntegration;
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
        rider.AddProducer<InviteCreatedEvent>("internal-retailer-wizardinvite");
        rider.AddProducer<BrandCreatedEvent>("internal-retailer-brandcreated");
        rider.AddProducer<UserCreatedEvent>("internal-retailer-usercreated");
        rider.AddProducer<EmailSentEvent>("internal-retailer-emailsent");

        //configure consumers
        rider.AddConsumer<InviteCreatedConsumer>();
        rider.AddConsumer<BrandCreatedConsumer>();
        rider.AddConsumer<SendEmailConsumer>();
        rider.AddConsumer<UserCreatedConsumer>();

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
            k.Host("localhost:9092");

            k.TopicEndpoint<string, InviteCreatedEvent>("internal-retailer-wizardinvite", "wizard-internal-consumer",
                e =>
                {
                    e.CreateIfMissing();
                    e.AutoOffsetReset = AutoOffsetReset.Latest;
                    e.ConfigureConsumer<InviteCreatedConsumer>(context);
                    e.ConfigureSaga<InviteState>(context);
                });

            k.TopicEndpoint<BrandCreatedEvent>("internal-retailer-brandcreated", "wizard-internal-consumer",
                e =>
                {
                    e.CreateIfMissing();
                    e.AutoOffsetReset = AutoOffsetReset.Latest;
                    e.ConfigureConsumer<BrandCreatedConsumer>(context);
                    e.ConfigureSaga<InviteState>(context);
                });

            k.TopicEndpoint<UserCreatedEvent>("internal-retailer-usercreated", "wizard-internal-consumer",
                e =>
                {
                    e.CreateIfMissing();
                    e.AutoOffsetReset = AutoOffsetReset.Latest;
                    e.ConfigureConsumer<UserCreatedConsumer>(context);
                    e.ConfigureSaga<InviteState>(context);
                });

            k.TopicEndpoint<EmailSentEvent>("internal-retailer-emailsent", "wizard-internal-consumer",
                e =>
                {
                    e.CreateIfMissing();
                    e.AutoOffsetReset = AutoOffsetReset.Latest;
                    e.ConfigureConsumer<SendEmailConsumer>(context);
                    e.ConfigureSaga<InviteState>(context);
                });
        });
    });
});

builder.Services.AddMediator(cfg =>
{
    cfg.AddConsumer<CreateInviteUseCase>();
    cfg.AddConsumer<CreateBrandUseCase>();
    cfg.AddConsumer<CreateUserUseCase>();
    cfg.AddConsumer<SendEmailUseCase>();
    cfg.AddConsumer<InviteStateProducerUseCase>();
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