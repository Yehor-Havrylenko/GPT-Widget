resource "aws_iam_role" "eks_node_group_role" {
  name               = "eks-node-group-role"
  assume_role_policy = jsonencode(local.iam_policies["eks_node_group_trust_policy"])
}
resource "aws_iam_role" "eks_cluster_role" {
  name               = "eks-cluster-role"
  assume_role_policy = jsonencode(local.iam_policies["eks_cluster_trust_policy"])
}