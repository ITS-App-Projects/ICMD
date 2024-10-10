{{/* vim: set filetype=mustache: */}}
{{/*
Expand the name of the chart.
*/}}
{{- define "app.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{/*
Create a default fully qualified app name.
We truncate at 63 chars because some Kubernetes name fields are limited to this (by the DNS naming spec).
If release name contains chart name it will be used as a full name.
*/}}
{{- define "app.fullname" -}}
{{- if .Values.fullnameOverride -}}
{{- .Values.fullnameOverride | trunc 63 | trimSuffix "-" -}}
{{- else -}}
{{- $name := default .Chart.Name .Values.nameOverride -}}
{{- if contains $name .Release.Name -}}
{{- .Release.Name | trunc 63 | trimSuffix "-" -}}
{{- else -}}
{{- printf "%s-%s" .Release.Name $name | trunc 63 | trimSuffix "-" -}}
{{- end -}}
{{- end -}}
{{- end -}}

{{/*
Create chart name and version as used by the chart label.
*/}}
{{- define "app.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{/*
Create chart secret for credentials name.
*/}}
{{- define "app.secret" -}}
{{- (print .Chart.Name "-credentials") -}}
{{- end -}}

{{/*
Create chart configmap for environments.
*/}}
{{- define "app.configmap" -}}
{{- (print .Chart.Name "-configmap") -}}
{{- end -}}

{{/*
Create chart name for ingress name.
*/}}
{{- define "app.ingress" -}}
{{- (print .Chart.Name "-ingress") -}}
{{- end -}}

{{/*
Create chart name for deployment name.
*/}}
{{- define "app.deployment" -}}
{{- (print .Chart.Name "-deployment") -}}
{{- end -}}