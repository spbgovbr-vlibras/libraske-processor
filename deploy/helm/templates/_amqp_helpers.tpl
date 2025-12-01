{{- define "libraskeapiGetAmqpUsername" -}}
{{- if and .Values.global .Values.global.amqp .Values.global.amqp.username (not .Values.externalServices.libraskeapi.amqp.username) }}
  {{- .Values.global.amqp.username -}}
{{- else if .Values.externalServices.libraskeapi.amqp.username -}}
  {{- .Values.externalServices.libraskeapi.amqp.username -}}
{{- else if .Values.rabbitmqha.rabbitmqUsername -}}
  {{- .Values.rabbitmqha.rabbitmqUsername -}}
{{- else -}}
  {{- "default-user" -}}
{{- end -}}
{{- end -}}

{{- define "libraskeapiGetAmqpPassword" -}}
{{- if and .Values.global .Values.global.amqp .Values.global.amqp.password (not .Values.externalServices.libraskeapi.amqp.password) }}
  {{- .Values.global.amqp.password -}}
{{- else if .Values.externalServices.libraskeapi.amqp.password -}}
  {{- .Values.externalServices.libraskeapi.amqp.password -}}
{{- else if .Values.rabbitmqha.rabbitmqPassword -}}
  {{- .Values.rabbitmqha.rabbitmqPassword -}}
{{- else -}}
  {{- "default-password" -}}
{{- end -}}
{{- end -}}

{{- define "libraskeapiGetAmqpHost" -}}
{{- if and .Values.global .Values.global.amqp .Values.global.amqp.host (not .Values.externalServices.libraskeapi.amqp.host) }}
  {{- .Values.global.amqp.host -}}
{{- else if .Values.externalServices.libraskeapi.amqp.host -}}
  {{- .Values.externalServices.libraskeapi.amqp.host -}}
{{- else if .Values.amqp.host -}}
  {{- .Values.amqp.host -}}
{{- else -}}
  {{- "localhost" -}}
{{- end -}}
{{- end -}}

{{- define "libraskeapiGetAmqpPort" -}}
{{- if and .Values.global .Values.global.amqp .Values.global.amqp.port (not .Values.externalServices.libraskeapi.amqp.port) }}
  {{- .Values.global.amqp.port | toString -}}
{{- else if .Values.externalServices.libraskeapi.amqp.port -}}
  {{- .Values.externalServices.libraskeapi.amqp.port | toString -}}
{{- else if .Values.amqp.port -}}
  {{- .Values.amqp.port | toString -}}
{{- else -}}
  {{- "5432" -}}
{{- end -}}
{{- end -}}

{{/*
Define the name of the PostgreSQL secret to use.
*/}}
{{- define "libraskeapiGetAmqpSecretName" -}}
{{- if and .Values.global .Values.global.amqp .Values.global.amqp.existingSecrets (not .Values.externalServices.libraskeapi.amqp.existingSecrets) }}
  {{- print .Values.global.amqp.existingSecrets }}
{{- else if .Values.externalServices.libraskeapi.amqp.existingSecrets }}
  {{- print .Values.externalServices.libraskeapi.amqp.existingSecrets }}
{{- else }}
  {{- print (include "libraskeapi.fullname" .) "-amqp-credentials" }}
{{- end }}
{{- end }}