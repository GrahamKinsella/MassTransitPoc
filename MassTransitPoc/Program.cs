using Confluent.Kafka;
using MassTransit;
using MassTransit.KafkaIntegration;
using MassTransit.KafkaIntegration.Configuration;
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
        rider.AddProducer<InviteCreatedEvent>("internal-development-retailer--wizardinvite");
        rider.AddProducer<BrandCreatedEvent>("internal-development-retailer--brandcreated");
        rider.AddProducer<UserCreatedEvent>("internal-development-retailer--usercreated");
        rider.AddProducer<EmailSentEvent>("internal-development-retailer--emailsent");

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
            k.TopicEndpoint<string, InviteCreatedEvent>("internal-development-retailer-wizardinvite", "development-wizard-internal-api",
                e =>
                {
                    e.CreateIfMissing();
                    e.ConfigureConsumer<InviteCreatedConsumer>(context);
                    e.ConfigureSaga<InviteState>(context);
                });

            k.TopicEndpoint<BrandCreatedEvent>("internal-development-retailer-brandcreated", "development-wizard-internal-api",
                e =>
                {
                    e.CreateIfMissing();
                    e.ConfigureConsumer<BrandCreatedConsumer>(context);
                    e.ConfigureSaga<InviteState>(context);
                });

            k.TopicEndpoint<UserCreatedEvent>("internal-development-retailer-usercreated", "development-wizard-internal-api",
                e =>
                {
                    e.CreateIfMissing();
                    e.ConfigureConsumer<UserCreatedConsumer>(context);
                    e.ConfigureSaga<InviteState>(context);
                });

            k.TopicEndpoint<EmailSentEvent>("internal-development-retailer-emailsent", "development-wizard-internal-api",
                e =>
                {
                    e.CreateIfMissing();
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