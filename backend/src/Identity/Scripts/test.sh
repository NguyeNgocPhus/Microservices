echo "phuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuu"
# Validate environment
if [ "$env" != "development" ] && [ "$env" != "production" ] 
then
  echo "-e Environment must be development or production"
fi