{{- define "libraskeprocessorGetPostgresDatabase" -}}
{{- if and .Values.global .Values.global.postgresql .Values.global.postgresql.dbName (not .Values.externalServices.libraskeprocessor.postgresql.dbName) }}
  {{- .Values.global.postgresql.dbName -}}
{{- else if .Values.externalServices.libraskeprocessor.postgresql.dbName -}}
  {{- .Values.externalServices.libraskeprocessor.postgresql.dbName -}}
{{- else if .Values.postgresql.auth.database -}}
  {{- .Values.postgresql.auth.database -}}
{{- else -}}
  {{- "default-db" -}}
{{- end -}}
{{- end -}}

{{- define "libraskeprocessorGetPostgresUsername" -}}
{{- if and .Values.global .Values.global.postgresql .Values.global.postgresql.username (not .Values.externalServices.libraskeprocessor.postgresql.username) }}
  {{- .Values.global.postgresql.username -}}
{{- else if .Values.externalServices.libraskeprocessor.postgresql.username -}}
  {{- .Values.externalServices.libraskeprocessor.postgresql.username -}}
{{- else if .Values.postgresql.auth.username -}}
  {{- .Values.postgresql.auth.username -}}
{{- else -}}
  {{- "default-user" -}}
{{- end -}}
{{- end -}}

{{- define "libraskeprocessorGetPostgresPassword" -}}
{{- if and .Values.global .Values.global.postgresql .Values.global.postgresql.password (not .Values.externalServices.libraskeprocessor.postgresql.password) }}
  {{- .Values.global.postgresql.password -}}
{{- else if .Values.externalServices.libraskeprocessor.postgresql.password -}}
  {{- .Values.externalServices.libraskeprocessor.postgresql.password -}}
{{- else if .Values.postgresql.auth.password -}}
  {{- .Values.postgresql.auth.password -}}
{{- else -}}
  {{- "default-password" -}}
{{- end -}}
{{- end -}}

{{- define "libraskeprocessorGetPostgresHost" -}}
{{- if and .Values.global .Values.global.postgresql .Values.global.postgresql.host (not .Values.externalServices.libraskeprocessor.postgresql.host) }}
  {{- .Values.global.postgresql.host -}}
{{- else if .Values.externalServices.libraskeprocessor.postgresql.host -}}
  {{- .Values.externalServices.libraskeprocessor.postgresql.host -}}
{{- else if .Values.postgresql.host -}}
  {{- .Values.postgresql.host -}}
{{- else -}}
  {{- "localhost" -}}
{{- end -}}
{{- end -}}

{{- define "libraskeprocessorGetPostgresPort" -}}
{{- if and .Values.global .Values.global.postgresql .Values.global.postgresql.port (not .Values.externalServices.libraskeprocessor.postgresql.port) }}
  {{- .Values.global.postgresql.port | toString -}}
{{- else if .Values.externalServices.libraskeprocessor.postgresql.port -}}
  {{- .Values.externalServices.libraskeprocessor.postgresql.port | toString -}}
{{- else if .Values.postgresql.port -}}
  {{- .Values.postgresql.port | toString -}}
{{- else -}}
  {{- "5432" -}}
{{- end -}}
{{- end -}}

{{/*
Define the name of the PostgreSQL secret to use.
*/}}
{{- define "libraskeprocessorGetPostgresSecretName" -}}
{{- if and .Values.global .Values.global.postgresql .Values.global.postgresql.existingSecrets (not .Values.externalServices.libraskeprocessor.postgresql.existingSecrets) }}
  {{- print .Values.global.postgresql.existingSecrets }}
{{- else if .Values.externalServices.libraskeprocessor.postgresql.existingSecrets }}
  {{- print .Values.externalServices.libraskeprocessor.postgresql.existingSecrets }}
{{- else }}
  {{- print (include "libraskeprocessor.fullname" .) "-db-credentials" }}
{{- end }}
{{- end }}
