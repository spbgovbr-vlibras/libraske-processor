{{- define "libraskeapiGetPostgresDatabase" -}}
{{- if and .Values.global .Values.global.postgresql .Values.global.postgresql.dbName (not .Values.externalServices.libraskeapi.postgresql.dbName) }}
  {{- .Values.global.postgresql.dbName -}}
{{- else if .Values.externalServices.libraskeapi.postgresql.dbName -}}
  {{- .Values.externalServices.libraskeapi.postgresql.dbName -}}
{{- else if .Values.postgresql.auth.database -}}
  {{- .Values.postgresql.auth.database -}}
{{- else -}}
  {{- "default-db" -}}
{{- end -}}
{{- end -}}

{{- define "libraskeapiGetPostgresUsername" -}}
{{- if and .Values.global .Values.global.postgresql .Values.global.postgresql.username (not .Values.externalServices.libraskeapi.postgresql.username) }}
  {{- .Values.global.postgresql.username -}}
{{- else if .Values.externalServices.libraskeapi.postgresql.username -}}
  {{- .Values.externalServices.libraskeapi.postgresql.username -}}
{{- else if .Values.postgresql.auth.username -}}
  {{- .Values.postgresql.auth.username -}}
{{- else -}}
  {{- "default-user" -}}
{{- end -}}
{{- end -}}

{{- define "libraskeapiGetPostgresPassword" -}}
{{- if and .Values.global .Values.global.postgresql .Values.global.postgresql.password (not .Values.externalServices.libraskeapi.postgresql.password) }}
  {{- .Values.global.postgresql.password -}}
{{- else if .Values.externalServices.libraskeapi.postgresql.password -}}
  {{- .Values.externalServices.libraskeapi.postgresql.password -}}
{{- else if .Values.postgresql.auth.password -}}
  {{- .Values.postgresql.auth.password -}}
{{- else -}}
  {{- "default-password" -}}
{{- end -}}
{{- end -}}

{{- define "libraskeapiGetPostgresHost" -}}
{{- if and .Values.global .Values.global.postgresql .Values.global.postgresql.host (not .Values.externalServices.libraskeapi.postgresql.host) }}
  {{- .Values.global.postgresql.host -}}
{{- else if .Values.externalServices.libraskeapi.postgresql.host -}}
  {{- .Values.externalServices.libraskeapi.postgresql.host -}}
{{- else if .Values.postgresql.host -}}
  {{- .Values.postgresql.host -}}
{{- else -}}
  {{- "localhost" -}}
{{- end -}}
{{- end -}}

{{- define "libraskeapiGetPostgresPort" -}}
{{- if and .Values.global .Values.global.postgresql .Values.global.postgresql.port (not .Values.externalServices.libraskeapi.postgresql.port) }}
  {{- .Values.global.postgresql.port | toString -}}
{{- else if .Values.externalServices.libraskeapi.postgresql.port -}}
  {{- .Values.externalServices.libraskeapi.postgresql.port | toString -}}
{{- else if .Values.postgresql.port -}}
  {{- .Values.postgresql.port | toString -}}
{{- else -}}
  {{- "5432" -}}
{{- end -}}
{{- end -}}

{{/*
Define the name of the PostgreSQL secret to use.
*/}}
{{- define "libraskeapiGetPostgresSecretName" -}}
{{- if and .Values.global .Values.global.postgresql .Values.global.postgresql.existingSecrets (not .Values.externalServices.libraskeapi.postgresql.existingSecrets) }}
  {{- print .Values.global.postgresql.existingSecrets }}
{{- else if .Values.externalServices.libraskeapi.postgresql.existingSecrets }}
  {{- print .Values.externalServices.libraskeapi.postgresql.existingSecrets }}
{{- else }}
  {{- print (include "libraskeapi.fullname" .) "-db-credentials" }}
{{- end }}
{{- end }}
