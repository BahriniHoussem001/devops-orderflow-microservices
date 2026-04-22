# Kubernetes manifests

Ce dossier contient les manifests Kubernetes du projet DevOps OrderFlow.

## Fichiers
- `namespace.yaml` : création du namespace `orderflow`
- `order-postgres.yaml` : base PostgreSQL du microservice Order
- `notification-postgres.yaml` : base PostgreSQL du microservice Notification
- `rabbitmq.yaml` : broker RabbitMQ
- `order-service.yaml` : déploiement du microservice Order
- `notification-service.yaml` : déploiement du microservice Notification
- `order-service-nodeport.yaml` : exposition externe du microservice Order
- `notification-service-nodeport.yaml` : exposition externe du microservice Notification
- `kustomization.yaml` : agrégation des manifests Kubernetes
- `argocd-application.yaml` : configuration GitOps pour ArgoCD

## Déploiement
Tous les manifests peuvent être appliqués ensemble avec :

```bash
kubectl apply -k k8s