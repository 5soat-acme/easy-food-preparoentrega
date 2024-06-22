@MonitorController
Feature: MonitorController

   Controller de Monitor

Scenario: Obtendo pedidos para monitoramento
	Given que eu tenho pedidos para monitoramento
	When eu solicitar os pedidos para monitoramento
	Then a resposta deve ser 200

Scenario: Não há pedidos para monitoramento
	Given que não existem pedidos para monitoramento
	When eu solicitar os pedidos para monitoramento
	Then a resposta deve ser 404