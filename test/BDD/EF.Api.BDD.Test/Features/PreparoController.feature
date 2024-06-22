@PreparoController
Feature: PreparoController

   Controller de Preparo

Scenario: Obter um pedido existente
	Given o pedido com id "f694f3a3-2622-45ea-b168-f573f16165ea" existe
	When eu solicitar o pedido com id "f694f3a3-2622-45ea-b168-f573f16165ea"
	Then a resposta deve ser 200

Scenario: Obtendo pedidos por status
	Given que existem pedidos com diferentes status
	When eu solicitar pedidos com status "Recebido"
	Then a resposta deve ser 200

Scenario: Iniciando a preparação de um pedido
	Given o pedido com id "f694f3a3-2622-45ea-b168-f573f16165ea" existe
	When eu enviar uma solicitação para iniciar a preparação
	Then a resposta deve ser 204

Scenario: Finalizando a preparação de um pedido
	Given o pedido com id "f694f3a3-2622-45ea-b168-f573f16165ea" existe
	When eu enviar uma solicitação para finalizar a preparação
	Then a resposta deve ser 204

Scenario: Confirmando a entrega de um pedido
	Given o pedido com id "f694f3a3-2622-45ea-b168-f573f16165ea" existe
	When eu enviar uma solicitação para confirmar a entrega
	Then a resposta deve ser 204