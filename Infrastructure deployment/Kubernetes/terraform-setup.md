# Terraform Setup for EKS

If you don't have a Kubernetes cluster set up, you can use Terraform to create an EKS cluster. Follow these steps:

### 1. Initialize Terraform

Navigate to the `terraform` directory and run:

```bash
terraform init
```
### 2. Apply Terraform Configuration
Apply the Terraform configuration to create the necessary AWS resources:

```bash
terraform apply
```
Confirm any prompts to proceed with the infrastructure creation.
### 3. Configure kubectl
Once Terraform completes, update your kubectl configuration to use the new EKS cluster:
```bash
aws eks --region eu-central-1 update-kubeconfig --name my-eks-cluster
```
Your EKS cluster is now ready, and you can proceed with [Helm deployment](Setup.md)