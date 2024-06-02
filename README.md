# Easy Food - Microsserviço Preparo e Entrega

Microsserviço de **Preparo e Entrega** referente ao projeto **[Easy Food](https://github.com/5soat-acme/easy-food)**.

## Workflow - Github Action :arrow_forward:

### Pré-requisitos :clipboard:
- Para o correto funcionamento do workflow é necessário configurar as seguintes secrets no repositório, de acordo com a conta da AWS Academy e da conta do Docker Hub:
    - AWS_ACCESS_KEY_ID
    - AWS_SECRET_ACCESS_KEY
    - AWS_SESSION_TOKEN
    - DOCKERHUB_TOKEN
- Ter a infraestrutura dos seguintes repositórios já criadas:
    - [easy-food-infra](https://github.com/5soat-acme/easy-food-infra)
    - [easy-food-infra-database](https://github.com/5soat-acme/easy-food-infra-database)
    - [easy-food-lambda](https://github.com/5soat-acme/easy-food-lambda)

### Executando :running:
- O repositório conta com um workflow disparado quando houver **push** na branch **main**. O workflow é utilizado para: 
    - Criar a imagem da API e publicar no **[Docker Hub](https://hub.docker.com/r/5soatacme/easy-food-preparoentrega)**.
    - Executar ```rollout restart``` no deployment do cluster EKS criado pelo repositório **[easy-food-infra](https://github.com/5soat-acme/easy-food-infra)**.