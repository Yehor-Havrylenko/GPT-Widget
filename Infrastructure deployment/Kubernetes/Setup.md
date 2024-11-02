# Setup Instruction

## 1. Set secret values
In `Helm/values.yaml`, you need to set the following values:

```
  OPENAI_ASSISTANT_ID: "your_assistant_id"
  OPENAI_KEY: "your_openai_key"
```
## 2. Start Helm
Run the following command to install the Helm chart:
```js
helm install <release-name> ./Helm
```
## 3. Visit your site

You can check the domain or external IP of your site using this command:
```bash
kubectl get svc frontend -o wide
```
#### Note: If you don’t have Kubernetes set up, [Terraform setup instructions](terraform-setup.md)