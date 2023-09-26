terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 4.0"
    }
  }
}

# Configure the AWS Provider
provider "aws" {
  region = "eu-west-1"
}

resource "random_uuid" "bucket_random_id" {
}

# Create S3 bucket to store our application source code.
resource "aws_s3_bucket" "lambda_bucket" {
  bucket        = "${random_uuid.bucket_random_id.result}-leaderboard-tf-bucket"
  acl           = "private"
  force_destroy = true
}

# Create a zip file of our application source code.
data "archive_file" "lambda_archive" {
  type = "zip"

  source_dir  = "App/src/LeaderBoard/bin/Release/net6.0/linux-x64/publish"
  output_path = "LeaderBoard.zip"
}

# Upload the zip file to the S3 bucket.
resource "aws_s3_object" "lambda_bundle" {
  bucket = aws_s3_bucket.lambda_bucket.id

  key    = "LeaderBoard.zip"
  source = data.archive_file.lambda_archive.output_path

  etag = filemd5(data.archive_file.lambda_archive.output_path)
}

# Create a CloudWatch log group for our Lambda function
resource "aws_cloudwatch_log_group" "aggregator" {
  name = "/aws/lambda/${aws_lambda_function.function.function_name}"

  retention_in_days = 30
}

resource "aws_lambda_function" "function" {
  function_name    = "leaderboard-lambda"
  s3_bucket        = aws_s3_bucket.lambda_bucket.id
  s3_key           = aws_s3_object.lambda_bundle.key
  runtime          = "dotnet6"
  handler          = "LeaderBoard"
  source_code_hash = data.archive_file.lambda_archive.output_base64sha256
  role             = aws_iam_role.lambda_function_role.arn
  timeout          = 30
}

resource "aws_dynamodb_table" "leaderboard" {
  name             = "leaderboard"
  billing_mode     = "PROVISIONED"
  read_capacity    = "30"
  write_capacity   = "30"
  attribute {
    name = "Pk"
    type = "S"
  }
  attribute {
    name = "Score"
    type = "N"
  }
  attribute {
    name = "UserId"
    type = "S"
  }
  hash_key  = "Pk"
  range_key = "Score"
  global_secondary_index {
    name            = "user-index"
    hash_key        = "UserId"
    projection_type = "ALL"
    read_capacity    = "30"
    write_capacity   = "30"
  }
}


resource "aws_iam_role_policy_attachment" "lambda_policy_attach" {
  role       = aws_iam_role.lambda_function_role.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole"
}

resource "aws_iam_role" "lambda_function_role" {
  name = "FunctionIamRole_leaderBoard-lambda"
  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [{
      Action = "sts:AssumeRole"
      Effect = "Allow"
      Sid    = ""
      Principal = {
        Service = "lambda.amazonaws.com"
      }
      }
    ]
  })
}

resource "aws_iam_role_policy" "dynamodb_lambda_policy" {
  name   = "lambda-dynamodb-policy"
  role   = aws_iam_role.lambda_function_role.id
  policy = <<EOF
{
  "Version": "2012-10-17",
  "Statement": [
    {
        "Sid": "AllowLambdaFunctionToCreateLogs",
        "Action": [ 
            "logs:*" 
        ],
        "Effect": "Allow",
        "Resource": [ 
            "arn:aws:logs:*:*:*" 
        ]
    },
    {
        "Sid": "AllowLambdaToWriteToDynamoDB",
        "Effect": "Allow",
        "Action": [
            "dynamodb:BatchWriteItem",
            "dynamodb:PutItem"
        ],
        "Resource": "${aws_dynamodb_table.leaderboard.arn}"
    }
  ]
}
EOF
}

resource "aws_lambda_function_url" "lambda_function_url" {
  function_name      = aws_lambda_function.function.function_name
  authorization_type = "NONE"
}

output "lambda_function_details" {
  value = aws_lambda_function.function.arn
}

output "lambda_function_url" {
  value = aws_lambda_function_url.lambda_function_url.function_url
}