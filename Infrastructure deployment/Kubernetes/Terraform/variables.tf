variable "region" {
  description = "AWS region for the deployment"
  default     = "eu-central-1"
}

variable "cluster_name" {
  description = "Name of the EKS cluster"
  default     = "my-eks-cluster"
}

variable "vpc_cidr" {
  description = "CIDR block for the VPC"
  default     = "10.0.0.0/16"
}

variable "public_subnets" {
  description = "List of public subnet CIDR blocks"
  type        = list(string)
  default     = ["10.0.1.0/24", "10.0.2.0/24"]
}

variable "private_subnets" {
  description = "List of private subnet CIDR blocks"
  type        = list(string)
  default     = ["10.0.101.0/24", "10.0.102.0/24"]
}

variable "availability_zones" {
  description = "List of availability zones"
  type        = list(string)
  default     = ["eu-central-1a", "eu-central-1b"]
}

variable "desired_size" {
  description = "Desired number of worker nodes"
  default     = 2
}

variable "min_size" {
  description = "Minimum number of worker nodes"
  default     = 1
}

variable "max_size" {
  description = "Maximum number of worker nodes"
  default     = 3
}

variable "instance_type" {
  description = "EC2 instance type for worker nodes"
  default     = "t3.medium"
}
