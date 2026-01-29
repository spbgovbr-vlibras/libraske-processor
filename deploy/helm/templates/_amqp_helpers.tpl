{{- define "libraskeprocessorGetAmqpUsername" -}}
{{- if and .Values.global .Values.global.amqp .Values.global.amqp.username (not .Values.externalServices.libraskeprocessor.amqp.username) }}
  {{- .Values.global.amqp.username -}}
{{- else if .Values.externalServices.libraskeprocessor.amqp.username -}}
  {{- .Values.externalServices.libraskeprocessor.amqp.username -}}
{{- else if .Values.rabbitmqha.rabbitmqUsername -}}
  {{- .Values.rabbitmqha.rabbitmqUsername -}}
{{- else -}}
  {{- "default-user" -}}
{{- end -}}
{{- end -}}

{{- define "libraskeprocessorGetAmqpPassword" -}}
{{- if and .Values.global .Values.global.amqp .Values.global.amqp.password (not .Values.externalServices.libraskeprocessor.amqp.password) }}
  {{- .Values.global.amqp.password -}}
{{- else if .Values.externalServices.libraskeprocessor.amqp.password -}}
  {{- .Values.externalServices.libraskeprocessor.amqp.password -}}
{{- else if .Values.rabbitmqha.rabbitmqPassword -}}
  {{- .Values.rabbitmqha.rabbitmqPassword -}}
{{- else -}}
  {{- "default-password" -}}
{{- end -}}
{{- end -}}

{{- define "libraskeprocessorGetAmqpHost" -}}
{{- if and .Values.global .Values.global.amqp .Values.global.amqp.host (not .Values.externalServices.libraskeprocessor.amqp.host) }}
  {{- .Values.global.amqp.host -}}
{{- else if .Values.externalServices.libraskeprocessor.amqp.host -}}
  {{- .Values.externalServices.libraskeprocessor.amqp.host -}}
{{- else if .Values.amqp.host -}}
  {{- .Values.amqp.host -}}
{{- else -}}
  {{- "localhost" -}}
{{- end -}}
{{- end -}}

{{- define "libraskeprocessorGetAmqpPort" -}}
{{- if and .Values.global .Values.global.amqp .Values.global.amqp.port (not .Values.externalServices.libraskeprocessor.amqp.port) }}
  {{- .Values.global.amqp.port | toString -}}
{{- else if .Values.externalServices.libraskeprocessor.amqp.port -}}
  {{- .Values.externalServices.libraskeprocessor.amqp.port | toString -}}
{{- else if .Values.amqp.port -}}
  {{- .Values.amqp.port | toString -}}
{{- else -}}
  {{- "5432" -}}
{{- end -}}
{{- end -}}

{{/*
Define the name of the PostgreSQL secret to use.
*/}}
{{- define "libraskeprocessorGetAmqpSecretName" -}}
{{- if and .Values.global .Values.global.amqp .Values.global.amqp.existingSecrets (not .Values.externalServices.libraskeprocessor.amqp.existingSecrets) }}
  {{- print .Values.global.amqp.existingSecrets }}
{{- else if .Values.externalServices.libraskeprocessor.amqp.existingSecrets }}
  {{- print .Values.externalServices.libraskeprocessor.amqp.existingSecrets }}
{{- else }}
  {{- print (include "libraskeprocessor.fullname" .) "-amqp-credentials" }}
{{- end }}
{{- end }}